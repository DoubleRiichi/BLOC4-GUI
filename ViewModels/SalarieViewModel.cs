

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Bloc4_GUI.Models;
using Bloc4_GUI.placeholder;
using ReactiveUI;

namespace Bloc4_GUI.ViewModels;



public class SalarieViewModel : ReactiveObject, IRoutableViewModel
{
        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = "Salaries";
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

        public ObservableCollection<string> ServicesDropdown { get; }
        public ObservableCollection<string> SitesDropdown { get; }

        public ReactiveCommand<Unit, Unit> FilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }


        public SalarieViewModel(IScreen screen) {
            HostScreen = screen;

            FilterCommand = ReactiveCommand.Create(Filter);
            ResetFilterCommand = ReactiveCommand.Create(ResetFilter);
            
            Salaries = new ObservableCollection<Salarie>(BaseSalaries);
            
            ServicesDropdown = new ObservableCollection<string>(BaseSalaries
                .Select(s => s.service.nom)
                .Distinct()
                .ToList());

            // ServicesDropdown.Insert(0, " ");
            
            SitesDropdown = new ObservableCollection<string>(BaseSalaries
                .Select(s => s.service.site.nom)
                .Distinct()
                .ToList());
            
            // SitesDropdown.Insert(0, " ");
            
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
}