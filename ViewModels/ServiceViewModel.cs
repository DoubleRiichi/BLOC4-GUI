using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Bloc4_GUI.DTO;
using Bloc4_GUI.Models;
using Bloc4_GUI.Services;
using Bloc4_GUI.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Net.Http;


namespace Bloc4_GUI.ViewModels;

public class ServiceViewModel :  ReactiveObject, IRoutableViewModel {
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = "Services";

        private string _selectedService;
        public string SelectedService {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }
        
        private string _selectedSite;
        
        public string SelectedSite {
            get => _selectedSite;
            set => this.RaiseAndSetIfChanged(ref _selectedSite, value);
        }


        public bool Connected
        {
            get => AuthService.Connected;
        }

        public bool NotConnected
        {
            get => !AuthService.Connected;
        }

    public Service? currentCellState { get; set; }

        public ObservableCollection<Service> BaseServices { get; set; }
        
        public ObservableCollection<Service> Services { get; set; }
        public ObservableCollection<Service> PageServices { get; set; }
        public AvaloniaDictionary<int, Service> ServicesToBeDeleted { get; set; }
        public List<Site> Sites {get; set;} 

        public ObservableCollection<string> ServicesDropdown { get; set; }
        public ObservableCollection<string> SitesDropdown { get; set; }

        public ReactiveCommand<Unit, Unit> FilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }
        public ReactiveCommand<Unit, Task> ConfirmChangesCommand { get; }
        public ReactiveCommand<Unit, Task> CancelChangesCommand { get; }  
        public ReactiveCommand<Unit, Unit> GoToPreviousPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToNextPageCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenAddServiceCommand { get; }

        private int ItemsPerPage = 30;
        public int TotalPages;
        public List<int> PagesIndex {get; set;} = new List<int>(1);

        private bool _canGoToPreviousPage = false;
        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set => this.RaiseAndSetIfChanged(ref _canGoToPreviousPage, value);
        }

        private bool _canGoToNextPage = false;
        public bool CanGoToNextPage
            {
                get => _canGoToNextPage;
                set => this.RaiseAndSetIfChanged(ref _canGoToNextPage, value);
            }

        private int _currentPage = 1;
        public int CurrentPage 
        {
            get => _currentPage;
            set  {    
                        this.RaiseAndSetIfChanged(ref _currentPage, value);
                       
                     }
        }

        private int _pageSelector = 1;
        public int PageSelector {
            get => _pageSelector;
            set {   
                    this.RaiseAndSetIfChanged(ref _pageSelector, value);
                    GoToPage(_pageSelector);
            }
        }



        public ServiceViewModel(IScreen screen) {
            HostScreen = screen;

            FilterCommand = ReactiveCommand.Create(Filter);
            ResetFilterCommand = ReactiveCommand.Create(ResetFilter);
            ConfirmChangesCommand = ReactiveCommand.Create(ConfirmChangesAsync);
            CancelChangesCommand = ReactiveCommand.Create(CancelChangesAsync);
            GoToPreviousPageCommand = ReactiveCommand.Create(GoToPreviousPage);
            GoToNextPageCommand = ReactiveCommand.Create(GoToNextPage);
            OpenAddServiceCommand = ReactiveCommand.Create(OpenAddService);


            
            Services = new ObservableCollection<Service>();
            Sites = new List<Site>();
            ServicesDropdown = new ObservableCollection<string>();
            SitesDropdown = new ObservableCollection<string>();
        }


        public async Task InitializeAsync() {
            try {
                var servicesApi = await ApiService.GetAsync<List<Service>>("Services/get");
                Sites.AddRange(await ApiService.GetAsync<List<Site>>("Sites/get"));

                BaseServices = new ObservableCollection<Service>(servicesApi); 
                    

                foreach(var service in BaseServices) {
                    Services.Add(service.Clone());    
                }

                PageServices = new ObservableCollection<Service>(Services.Take(ItemsPerPage));
                
                TotalPages = (int)Math.Ceiling((double)Services.Count / ItemsPerPage);
                updatePagesIndex();
                CanGoToPreviousPage = checkGoToPreviousPage();
                CanGoToNextPage = checkGoToNextPage();

                ServicesDropdown.AddRange(BaseServices
                    .Select(s => s.nom)
                    .Distinct()
                    .ToList());

                // ServicesDropdown.Insert(0, " ");
                
                SitesDropdown.AddRange(Sites
                    .Select(s => s.nom)
                    .Distinct()
                    .ToList());    
            } catch (Exception ex) {
                var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Le logiciel ne peut pas contacter l'API, si le problème persiste consulter votre support informatique.", ButtonEnum.Ok);
                box.ShowWindowAsync();
            }
                

        }

        private bool checkGoToPreviousPage() => _currentPage > 1;

        private bool checkGoToNextPage() => _currentPage < TotalPages;

        private void GoToPreviousPage()
        {
                PageSelector--;
                CanGoToPreviousPage = checkGoToPreviousPage();
                CanGoToNextPage = checkGoToNextPage();
        }

        private void GoToNextPage()
        {
                PageSelector++;
                CanGoToPreviousPage = checkGoToPreviousPage();
                CanGoToNextPage = checkGoToNextPage();
        }

        public void GoToPage(int pageNumber) {
                if (pageNumber < 1 || pageNumber > TotalPages)
                return;
                
                CurrentPage = pageNumber;
                UpdatePageServices();
        }

        private void UpdatePageServices() {
                var skipCount = (_currentPage - 1) * ItemsPerPage;

                var currentPageItems = Services.Skip(skipCount).Take(ItemsPerPage);

                PageServices.Clear();

                foreach (var item in currentPageItems) {
                PageServices.Add(item);
                } 
        }

        private void SetItemsPerPage(int ItemsPerPage) {
                if(ItemsPerPage > BaseServices.Count)
                return;
                
                this.ItemsPerPage = ItemsPerPage;
                TotalPages = (int)Math.Ceiling((double)Services.Count / ItemsPerPage);
                updatePagesIndex();
        }

        private void updatePagesIndex() {
                PagesIndex.Clear();
                TotalPages = (int)Math.Ceiling((double)Services.Count / ItemsPerPage);

                for(int i = 1; i <= TotalPages; i++) {
                    PagesIndex.Add(i);
                }
        }


    public void Filter() {
        Services.Clear();

        var filteredServices = BaseServices.AsEnumerable(); 

        if (!string.IsNullOrWhiteSpace(_selectedService))
        {
            filteredServices = filteredServices.Where(s => s.nom == _selectedService);
        }

        if (!string.IsNullOrWhiteSpace(_selectedSite))
        {
            filteredServices = filteredServices.Where(s => s.site.nom == _selectedSite);
        }

        foreach (var service in filteredServices)
        {
            Services.Add(service);
        }

        PageSelector = 1;
        updatePagesIndex();
    }

    public void ResetFilter() {
        Services.Clear();

        SelectedService = "";
        SelectedSite = "";


        foreach (var salarie in BaseServices) {
            Services.Add(salarie);
        }
    }

    public async Task ConfirmChangesAsync() {

        var editedService = Services
                                .Where(s => s.HasChanges == true)
                                .ToList();         

        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Cette opération entraine des changements en base de données, assurez vous qu'ils soient conforme à vos attentes.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();



        if (result == ButtonResult.Ok) {
            foreach (var service in editedService) {
                var site = Sites.Where(s => s.nom == service.site.nom).FirstOrDefault();
                service.site = site;
                
                var dto = new ServiceDTO(service);

                var error = false;

                try {
                    _ = await ApiService.PutAsync<ServiceDTO>("Services/update", new {services = dto, token = AuthService.GetInstance().token});
                } catch (HttpRequestException ex) {
                    Console.WriteLine(ex.Message);
                    error = true;
                }
                catch (Exception ex) { }

                if (error)
                {
                    box = MessageBoxManager.GetMessageBoxStandard("Info", "Erreur Réseau : Changements impossibles", ButtonEnum.Ok);
                    result = await box.ShowAsync();
                }
                else
                {
                    box = MessageBoxManager.GetMessageBoxStandard("Info", "Changements réussis", ButtonEnum.Ok);
                    result = await box.ShowAsync();
                }
            }
        }
    }

    public async Task CancelChangesAsync() {
        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes sur le point d'annuler vos changements.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();
        
        if(result == ButtonResult.Ok) {
            for(int i = 0; i < PageServices.Count ; i++) {

                if(PageServices[i].HasChanges) {
                    var baseService = BaseServices
                                        .FirstOrDefault(s => s.id == PageServices[i].id);
                    if(baseService != null) {
                        PageServices[i] = baseService.Clone();
                    
                    }
                }
            }    

            foreach(var item in ServicesToBeDeleted) {
                Services.Insert(item.Key, item.Value);
            }

            ServicesToBeDeleted.Clear();

            PageSelector = CurrentPage;
        }
    }


   public void SaveCellState(DataGridPreparingCellForEditEventArgs e) {
        var state = e.Row.DataContext as Service;
        currentCellState = state.Clone();
    }


    public async void HandleCellEdit(DataGridCellEditEndedEventArgs e) {
        var editedServices = e.Row.DataContext as Service;
  

        editedServices?.MarkAsChanged();
        Service salarie = Services.Where(s => s.id == editedServices?.id).First();
                
            int index = Services.IndexOf(salarie);
            Services[index] = editedServices?.Clone();
        
    }


    public async void DeleteService(Service item) {
        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes sur le point d'annuler tous vos changements.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();

        var error = false;

        if(item != null && result == ButtonResult.Ok) {
            try {
                _ = await ApiService.DeleteAsync<Service>("Services/delete/" + item.id);
            } catch (HttpRequestException ex) {
                error = true;
            }
            catch (Exception ex) { }

            if (error)
            {
                box = MessageBoxManager.GetMessageBoxStandard("Info", "Erreur Réseau : Suppression impossible", ButtonEnum.Ok);
                result = await box.ShowAsync();
                return;
            }
            else
            {
                box = MessageBoxManager.GetMessageBoxStandard("Info", "Suppression réussite", ButtonEnum.Ok);
                result = await box.ShowAsync();
            }

            for (int i = 0; i < Services.Count; i++) {
                if (Services[i].id == item.id) {
                    Services.RemoveAt(i);
                    updatePagesIndex();
                    PageSelector = CurrentPage;
                    break;
                }
            }
        }
    }

    private void OpenAddService() {
        var window = new AddServiceView{
            DataContext = new AddServiceViewModel()
        };
        
        var mainWindow = Locator.Current.GetService<MainWindow>();

        // var modalViewModel = (AddSalarieView) window.DataContext;
        window.ShowDialog(mainWindow);

        updatePagesIndex();
        PageSelector = CurrentPage;
    }    
}