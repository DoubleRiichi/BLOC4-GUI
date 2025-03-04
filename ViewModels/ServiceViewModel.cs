using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.placeholder;
using Bloc4_GUI.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;


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

        public Service? currentCellState { get; set; }



        public ObservableCollection<Service> BaseServices { get; } = PlaceholderData.PlaceholderServices;
        
        public ObservableCollection<Service> Services { get; set; }
        public ObservableCollection<Service> PageServices { get; set; }
        

        public ObservableCollection<string> ServicesDropdown { get; }
        public ObservableCollection<string> SitesDropdown { get; }

        public ReactiveCommand<Unit, Unit> FilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfirmChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelChangesCommand { get; }  
        public ReactiveCommand<Unit, Unit> GoToPreviousPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToNextPageCommand { get; }

        private int ItemsPerPage = 30;
        public int TotalPages;
        public List<int> PagesIndex {get; set;} = new List<int>();

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
            ConfirmChangesCommand = ReactiveCommand.Create(ConfirmChanges);
            CancelChangesCommand = ReactiveCommand.Create(CancelChanges);
            GoToPreviousPageCommand = ReactiveCommand.Create(GoToPreviousPage);
            GoToNextPageCommand = ReactiveCommand.Create(GoToNextPage);

            Services = new ObservableCollection<Service>();
            
            foreach(var service in BaseServices) {
                Services.Add(service.Clone());    
            }

            PageServices = new ObservableCollection<Service>(Services.Take(ItemsPerPage));
            
            TotalPages = (int)Math.Ceiling((double)Services.Count / ItemsPerPage);
            updatePagesIndex();
            CanGoToPreviousPage = checkGoToPreviousPage();
            CanGoToNextPage = checkGoToNextPage();

            ServicesDropdown = new ObservableCollection<string>(BaseServices
                .Select(s => s.nom)
                .Distinct()
                .ToList());

            // ServicesDropdown.Insert(0, " ");
            
            SitesDropdown = new ObservableCollection<string>(BaseServices
                .Select(s => s.site.nom)
                .Distinct()
                .ToList());    
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
                UpdatePageSalaries();
        }

        private void UpdatePageSalaries() {
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

    public void ConfirmChanges() {

        var editedSalaries = Services
                                .Where(s => s.HasChanges == true)
                                .ToList();         
    }

    public void CancelChanges() {
        
        for(int i = 0; i < PageServices.Count ; i++) {

            if(PageServices[i].HasChanges) {
                var baseSalarie = BaseServices
                                    .FirstOrDefault(s => s.id == PageServices[i].id);
                if(baseSalarie != null) {
                    PageServices[i] = baseSalarie.Clone();
                
                }
            }
        }    

        PageSelector = CurrentPage;
    }


   public void SaveCellState(DataGridPreparingCellForEditEventArgs e) {
        var state = e.Row.DataContext as Service;
        currentCellState = state.Clone();
    }


    public async void HandleCellEdit(DataGridCellEditEndedEventArgs e) {
        var editedSalaries = e.Row.DataContext as Service;
  

        editedSalaries?.MarkAsChanged();
        Service salarie = Services.Where(s => s.id == editedSalaries?.id).First();
                
            int index = Services.IndexOf(salarie);
            Services[index] = editedSalaries?.Clone();
        
    }
    

}