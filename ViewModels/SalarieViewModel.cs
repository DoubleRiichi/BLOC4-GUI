

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Bloc4_GUI.Models;
using Bloc4_GUI.placeholder;
using Bloc4_GUI.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Bloc4_GUI.ViewModels;



public class SalarieViewModel : ReactiveObject, IRoutableViewModel
{
        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = "Salaries";
        private string _selectedService;

        //Utilisé lorsqu'une cellule du tableau est en cours de modification, 
        //permets de revenir à l'état initial lors d'une erreur de validation
        public Salarie currentCellState { get; set; }
        private string _selectedSite;
        public string SelectedService {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }
        
        
        public string SelectedSite {
            get => _selectedSite;
            set => this.RaiseAndSetIfChanged(ref _selectedSite, value);
        }

        private string _inputNom;
        
        public string InputNom {
            get => _inputNom;
            set => this.RaiseAndSetIfChanged(ref _inputNom, value);
        }

        private string _inputPrenom;
        
        public string InputPrenom {
            get => _inputPrenom;
            set => this.RaiseAndSetIfChanged(ref _inputPrenom, value);
        }

        private string _inputEmail;
        
        public string InputEmail {
            get => _inputEmail;
            set => this.RaiseAndSetIfChanged(ref _inputEmail, value);
        }
        private string _inputPhoneFixe;
        
        public string InputPhoneFixe {
            get => _inputPhoneFixe;
            set => this.RaiseAndSetIfChanged(ref _inputPhoneFixe, value);
        }
        private string _inputPhoneMobile;
        
        public string InputPhoneMobile {
            get => _inputPhoneMobile;
            set => this.RaiseAndSetIfChanged(ref _inputPhoneMobile, value);
        }



        public ObservableCollection<Salarie> BaseSalaries { get; } = PlaceholderData.Salaries;
        public ObservableCollection<Salarie> Salaries { get; set;}
        public ObservableCollection<Salarie> PageSalaries { get; set;}

        
        private int ItemsPerPage = 30;
        public int TotalPages;
        public List<int> PagesIndex {get; set;} = new List<int>();

        public ObservableCollection<string> ServicesDropdown { get; }
        public ObservableCollection<string> SitesDropdown { get; }

        public ReactiveCommand<Unit, Unit> FilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfirmChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelChangesCommand { get; }  
        public ReactiveCommand<Unit, Unit> GoToPreviousPageCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToNextPageCommand { get; }



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

        public SalarieViewModel(IScreen screen) {
            HostScreen = screen;

            FilterCommand = ReactiveCommand.Create(Filter);
            ResetFilterCommand = ReactiveCommand.Create(ResetFilter);
            ConfirmChangesCommand = ReactiveCommand.Create(ConfirmChanges);
            CancelChangesCommand = ReactiveCommand.Create(CancelChanges);
            GoToPreviousPageCommand = ReactiveCommand.Create(GoToPreviousPage);
            GoToNextPageCommand = ReactiveCommand.Create(GoToNextPage);
            
            Salaries = new ObservableCollection<Salarie>();
            
            foreach(var salarie in BaseSalaries) {
                Salaries.Add(salarie.Clone());    
            }

            PageSalaries = new ObservableCollection<Salarie>(Salaries.Take(ItemsPerPage));
            
            TotalPages = (int)Math.Ceiling((double)Salaries.Count / ItemsPerPage);
            updatePagesIndex();
            CanGoToPreviousPage = checkGoToPreviousPage();
            CanGoToNextPage = checkGoToNextPage();

            ServicesDropdown = new ObservableCollection<string>(BaseSalaries
                .Select(s => s.service.nom)
                .Distinct()
                .ToList());

            // ServicesDropdown.Insert(0, " ");
            
            SitesDropdown = new ObservableCollection<string>(BaseSalaries
                .Select(s => s.service.site.nom)
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

        var currentPageItems = Salaries.Skip(skipCount).Take(ItemsPerPage);

        PageSalaries.Clear();

        foreach (var item in currentPageItems) {
            PageSalaries.Add(item);
        } 
    }

    private void SetItemsPerPage(int ItemsPerPage) {
        if(ItemsPerPage > BaseSalaries.Count)
            return;
        
        this.ItemsPerPage = ItemsPerPage;
        TotalPages = (int)Math.Ceiling((double)Salaries.Count / ItemsPerPage);
        updatePagesIndex();
    }

    private void updatePagesIndex() {
        PagesIndex.Clear();
        TotalPages = (int)Math.Ceiling((double)Salaries.Count / ItemsPerPage);

        for(int i = 1; i <= TotalPages; i++) {
            PagesIndex.Add(i);
        }
    }



    public void Filter() {
        Salaries.Clear();

        var filteredSalaries = BaseSalaries.AsEnumerable(); 

        if (!string.IsNullOrWhiteSpace(_selectedService))
        {
            filteredSalaries = filteredSalaries.Where(s => s.service.nom == _selectedService);
        }

        if (!string.IsNullOrWhiteSpace(_selectedSite))
        {
            filteredSalaries = filteredSalaries.Where(s => s.service.site.nom == _selectedSite);
        }

        if (!string.IsNullOrWhiteSpace(_inputNom))
        {
            filteredSalaries = filteredSalaries.Where(s => s.nom.Contains(_inputNom, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(_inputPrenom))
        {
            filteredSalaries = filteredSalaries.Where(s => s.prenom.Contains(_inputPrenom, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(_inputEmail))
        {
            filteredSalaries = filteredSalaries.Where(s => s.email.Contains(_inputEmail, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(_inputPhoneFixe))
        {
            filteredSalaries = filteredSalaries.Where(s => s.telephone_fixe.Contains(_inputPhoneFixe));
        }

        if (!string.IsNullOrWhiteSpace(_inputPhoneMobile))
        {
            filteredSalaries = filteredSalaries.Where(s => s.telephone_mobile.Contains(_inputPhoneMobile));
        }

        foreach (var salarie in filteredSalaries)
        {
            Salaries.Add(salarie);
        }

        PageSelector = 1;
        updatePagesIndex();
    }

    public void ResetFilter() {
        Salaries.Clear();

        InputNom = "";
        InputPrenom = "";
        InputEmail = "";
        InputPhoneFixe = "";
        InputPhoneMobile = ""; 

        SelectedService = "";
        SelectedSite = "";


        foreach (var salarie in BaseSalaries) {
            Salaries.Add(salarie);
        }
    }

    public void ConfirmChanges() {

        var editedSalaries = Salaries
                                .Where(s => s.HasChanges == true)
                                .ToList();         
    }

    public void CancelChanges() {
        
        for(int i = 0; i < PageSalaries.Count ; i++) {

            if(PageSalaries[i].HasChanges) {
                var baseSalarie = BaseSalaries
                                    .FirstOrDefault(s => s.id == PageSalaries[i].id);
                if(baseSalarie != null) {
                    PageSalaries[i] = baseSalarie.Clone();
                
                }
            }
        }    

        PageSelector = CurrentPage;
    }


    public void SaveCellState(DataGridPreparingCellForEditEventArgs e) {
        var state = e.Row.DataContext as Salarie;
        currentCellState = state.Clone();
    }

    public async void HandleCellEdit(DataGridCellEditEndedEventArgs e) {
        var editedSalaries = e.Row.DataContext as Salarie;
        
        string message = "";
        bool error = false;

        if(!ValidatorService.isValidName(editedSalaries.nom)) {
            message += $"{editedSalaries.nom} n'est pas un nom valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidName(editedSalaries.prenom)) {
            message += $"{editedSalaries.prenom} n'est pas un prenom valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidLandline(editedSalaries.telephone_fixe)) {
            message += $"{editedSalaries.telephone_fixe} n'est pas une ligne fixe valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidMobilePhoneNumber(editedSalaries.telephone_mobile)) {
            message += $"{editedSalaries.telephone_mobile} n'est pas un numéro de téléphone portable valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidEmail(editedSalaries.email)) {
            message += $"{editedSalaries.email} n'est pas une adresse mail valide!\n";
            error = true;
        }
        
        if(error) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", message, ButtonEnum.Ok);
            var result = await box.ShowAsync();

            if(currentCellState != null) {
                e.Row.DataContext = currentCellState.Clone();
                Salarie salarie = Salaries.Where(s => s.id == currentCellState.id).First();
                
                int index = Salaries.IndexOf(salarie);
                Salaries[index] = currentCellState.Clone();
            }
            
        } else {

            editedSalaries?.MarkAsChanged();
            Salarie salarie = Salaries.Where(s => s.id == editedSalaries?.id).First();
                
            int index = Salaries.IndexOf(salarie);
            Salaries[index] = editedSalaries?.Clone();
        } 

    }
    


    
}