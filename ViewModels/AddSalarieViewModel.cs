using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Bloc4_GUI.Models;
using DynamicData;
using Bloc4_GUI.Services;
using System.Collections.Generic;
using Bloc4_GUI.DTO;
using MsBox.Avalonia;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using System;
using Tmds.DBus.Protocol;
using System.Net.Http;

namespace Bloc4_GUI.ViewModels;


public class AddSalarieViewModel : ReactiveObject
{
    // public ReactiveCommand<Unit, Unit> ValidateCommand { get; }
    


    private Service _selectedService;
    public Service SelectedService {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }
        
    private Site _selectedSite;
    public Site SelectedSite {
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

    public ObservableCollection<Service> ServicesDropdown { get; }
    public ObservableCollection<Site> SitesDropdown { get; }

    public AddSalarieViewModel()
    {
        ServicesDropdown = new ObservableCollection<Service>();
        SitesDropdown = new ObservableCollection<Site>();  
    }

    public async void InitializeAsync() {
        try {
            ServicesDropdown.AddRange(await ApiService.GetAsync<List<Service>>("Services/get"));
            SitesDropdown.AddRange(await ApiService.GetAsync<List<Site>>("Sites/get"));
        } catch (Exception ex) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Le logiciel ne peut pas contacter l'API, si le problème persiste consulter votre support informatique.", ButtonEnum.Ok);
            box.ShowWindowAsync();            
        }
    }


    public async Task<bool> Validate() {
        string message = "";
        bool error = false;

        if(!ValidatorService.isValidName(InputNom)) {
            message += $"{InputNom} n'est pas un nom valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidName(InputPrenom)) {
            message += $"{InputPrenom} n'est pas un prenom valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidLandline(InputPhoneFixe)) {
            message += $"{InputPhoneFixe} n'est pas une ligne fixe valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidMobilePhoneNumber(InputPhoneMobile)) {
            message += $"{InputPhoneMobile} n'est pas un numéro de téléphone portable valide!\n";
            error = true;
        }
        if(!ValidatorService.isValidEmail(InputEmail)) {
            message += $"{InputEmail} n'est pas une adresse mail valide!\n";
            error = true;
        }

        if(SelectedService?.site?.id != SelectedSite?.id) {
            message += "Le service n'est pas présent sur le site sélectionné.\n";
            error = true;
        }

        if (error) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", message, ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return false;
        }

        return true;
    }


    public async void CreateSalarie() {
        bool validSalarie = await Validate();
        
        if(!validSalarie) {
            return;
        }

        var salarie = new Salarie();
        salarie.id = null;
        salarie.nom = InputNom;
        salarie.prenom = InputPrenom;
        salarie.telephone_fixe = InputPhoneFixe;
        salarie.telephone_mobile = InputPhoneMobile;
        salarie.email = InputEmail;
        salarie.service = SelectedService;

        var error = false;
        
        var dto = new SalarieDTO(salarie);

        try {
            await ApiService.PostAsync<SalarieDTO>("Salaries/create", new {salaries = dto, token = AuthService.GetInstance().token});
        } catch (HttpRequestException ex) {
            
            Console.WriteLine(ex.Message);
            error = true;
        }
        catch (Exception ex) { }

        if (error)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Info", "Erreur réseau lors de l'ajout du salarié", ButtonEnum.Ok);
            var result = await box.ShowAsync();
        } else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Info", "Salarié ajouté avec succès", ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }

    }

}
