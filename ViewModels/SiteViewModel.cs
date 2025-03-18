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
using DynamicData.Kernel;
using System.Net.Http;


namespace Bloc4_GUI.ViewModels;

public class SiteViewModel :  ReactiveObject, IRoutableViewModel {
    public IScreen HostScreen { get; }

    // Unique identifier for the routable view model.
    public string UrlPathSegment { get; } = "Sites";

    public Site? currentCellState { get; set; }

    private string _nameInput;
    public string InputNom {
        get => _nameInput;
        set => this.RaiseAndSetIfChanged(ref _nameInput, value);
    }

    public bool Connected
    {
        get => AuthService.Connected;
    }

    public bool NotConnected
    {
        get => !AuthService.Connected;
    }

    public ObservableCollection<Site> BaseSites { get; set; }
    public ObservableCollection<Site> Sites { get; set; }
    public ObservableCollection<Site> PageSites { get; set; }
    public AvaloniaDictionary<int, Site> SiteToBeDeleted { get; set; }
    public ReactiveCommand<Unit, Unit> FilterCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }
    public ReactiveCommand<Unit, Task> ConfirmChangesCommand { get; }
    public ReactiveCommand<Unit, Task> CancelChangesCommand { get; }  
    public ReactiveCommand<Unit, Unit> GoToPreviousPageCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToNextPageCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddSiteCommand { get; }

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

    public SiteViewModel(IScreen screen) {
        HostScreen = screen;

        FilterCommand = ReactiveCommand.Create(Filter);
        ResetFilterCommand = ReactiveCommand.Create(ResetFilter);
        ConfirmChangesCommand = ReactiveCommand.Create(ConfirmChangesAsync);
        CancelChangesCommand = ReactiveCommand.Create(CancelChangesAsync);
        GoToPreviousPageCommand = ReactiveCommand.Create(GoToPreviousPage);
        GoToNextPageCommand = ReactiveCommand.Create(GoToNextPage);
        OpenAddSiteCommand = ReactiveCommand.Create(OpenAddSite);


        Sites = new ObservableCollection<Site>();
    }


    public async Task InitializeAsync() {
        try {
            var sitesApi = await ApiService.GetAsync<List<Site>>("Sites/get");

            BaseSites = new ObservableCollection<Site>(sitesApi); 
                

            foreach(var site in BaseSites) {
                Sites.Add(site.Clone());    
            }

            PageSites = new ObservableCollection<Site>(Sites.Take(ItemsPerPage));
            
            TotalPages = (int)Math.Ceiling((double)Sites.Count / ItemsPerPage);
            updatePagesIndex();
            CanGoToPreviousPage = checkGoToPreviousPage();
            CanGoToNextPage = checkGoToNextPage();

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
            UpdatePageSites();
    }

    private void UpdatePageSites() {
            var skipCount = (_currentPage - 1) * ItemsPerPage;

            var currentPageItems = Sites.Skip(skipCount).Take(ItemsPerPage);

            PageSites.Clear();

            foreach (var item in currentPageItems) {
                PageSites.Add(item);
            } 
    }    

    private void updatePagesIndex() {
            PagesIndex.Clear();
            TotalPages = (int)Math.Ceiling((double)Sites.Count / ItemsPerPage);

            for(int i = 1; i <= TotalPages; i++) {
                PagesIndex.Add(i);
            }
    }

    public void Filter() {
        Sites.Clear();

        var filteredServices = BaseSites.AsEnumerable(); 

        if(!string.IsNullOrWhiteSpace(_nameInput)) {
            filteredServices = filteredServices.Where(s =>_nameInput.Contains(s.nom));
        }

        foreach (var service in filteredServices)
        {
            Sites.Add(service);
        }

        PageSelector = 1;
        updatePagesIndex();
    }

    public void ResetFilter() {
        Sites.Clear();

        InputNom = "";

        foreach (var site in BaseSites) {
            Sites.Add(site);
        }
        }
    public async Task ConfirmChangesAsync() {

        var editedSite = Sites
                            .Where(s => s.HasChanges == true)
                            .ToList();         

        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Cette opération entraine des changements en base de données, assurez vous qu'ils soient conforme à vos attentes.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();
        var error = false;

        if (result == ButtonResult.Ok) {
            foreach (var site in editedSite) {
                
                var dto = new SiteDTO(site);

                try {
                    _ = await ApiService.PutAsync<SiteDTO>("Sites/update", new {site = dto, token = AuthService.GetInstance().token});
                } catch (HttpRequestException ex) {
                    Console.WriteLine(ex.Message);
                    error = true;
                }
                catch (Exception ex) { }
            }


            if (error)
            {
                box = MessageBoxManager.GetMessageBoxStandard("Info", "Erreur Réseau : Changements impossibles", ButtonEnum.Ok);
                result = await box.ShowAsync();
            }
            else
            {
                box = MessageBoxManager.GetMessageBoxStandard("Info", "Changements réussits", ButtonEnum.Ok);
                result = await box.ShowAsync();
            }

        }

    }


    public async Task CancelChangesAsync() {
        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes sur le point d'annuler vos changements.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();
        
        if(result == ButtonResult.Ok) {
            for(int i = 0; i < PageSites.Count ; i++) {

                if(PageSites[i].HasChanges) {
                    var baseSite = PageSites
                                        .FirstOrDefault(s => s.id == PageSites[i].id);
                    if(baseSite != null) {
                        PageSites[i] = baseSite.Clone();
                    
                    }
                }
            }    

            foreach(var item in SiteToBeDeleted) {
                Sites.Insert(item.Key, item.Value);
            }

            SiteToBeDeleted.Clear();

            PageSelector = CurrentPage;
        }
    }

   public void SaveCellState(DataGridPreparingCellForEditEventArgs e) {
        var state = e.Row.DataContext as Site;
        currentCellState = state.Clone();
    }


    public async void HandleCellEdit(DataGridCellEditEndedEventArgs e) {
        var editedSites = e.Row.DataContext as Site;
  

        editedSites?.MarkAsChanged();
        Site site = Sites.Where(s => s.id == editedSites?.id).First();
                
            int index = Sites.IndexOf(site);
            Sites[index] = editedSites?.Clone();
    }


    public async void DeleteSite(Site item) {
        var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes sur le point d'annuler tous vos changements.\nConfirmer?", ButtonEnum.OkAbort);
        var result = await box.ShowAsync();

        var error = false;
        
        if(item != null && result == ButtonResult.Ok) {
            try {
                _ = await ApiService.DeleteAsync<Site>("Sites/delete/" + item.id);
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

            for (int i = 0; i < Sites.Count; i++) {
                if (Sites[i].id == item.id) {
                    Sites.RemoveAt(i);
                    updatePagesIndex();
                    PageSelector = CurrentPage;
                    break;
                }
            }
        }
    }

    private void OpenAddSite() {
        var window = new AddSiteView{
            DataContext = new AddSiteViewModel()
        };
        
        var mainWindow = Locator.Current.GetService<MainWindow>();

        // var modalViewModel = (AddSalarieView) window.DataContext;
        window.ShowDialog(mainWindow);

        updatePagesIndex();
        PageSelector = CurrentPage;
    }    
}