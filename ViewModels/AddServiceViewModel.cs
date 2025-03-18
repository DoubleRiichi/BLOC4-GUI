using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Bloc4_GUI.Models;
using Bloc4_GUI.Services;
using DynamicData;
using System.Collections.Generic;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;
using Bloc4_GUI.DTO;
using System;
using System.Net.Http;

namespace Bloc4_GUI.ViewModels;


public class AddServiceViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> CreateServiceCommand { get; }
    
        
    private Site _selectedSite;
    public Site SelectedSite {
        get => _selectedSite;
        set => this.RaiseAndSetIfChanged(ref _selectedSite, value);
    }

    private string _inputNom;
    
    public string InputService {
        get => _inputNom;
        set => this.RaiseAndSetIfChanged(ref _inputNom, value);
    }

    public ObservableCollection<Site> SitesDropdown { get; set; }

    public AddServiceViewModel()
    {
        CreateServiceCommand = ReactiveCommand.Create(CreateService);
        SitesDropdown = new ObservableCollection<Site>();  
    }


    

    public async void InitializeAsync() {
        try {
            SitesDropdown.AddRange(await ApiService.GetAsync<List<Site>>("Sites/get"));
        } catch (Exception ex) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Le logiciel ne peut pas contacter l'API, si le problème persiste consulter votre support informatique.", ButtonEnum.Ok);
            box.ShowWindowAsync();            
        }
    }


    public async Task<bool> Validate() {
        string message = "";
        bool error = false;

        if(!ValidatorService.isValidName(InputService)) {
            message += $"{InputService} n'est pas un nom valide pour un service!\n";
            error = true;
        }

        if (error) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", message, ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return false;
        }

        return true;
    }


    public async void CreateService() {
        bool validService = await Validate();

        if (!validService) {
            return;
        }

        var dto = new ServiceDTO(
            new Service {
            id = null,
            nom = InputService,
            site = SelectedSite            
        });

        var error = false;

        try {
            await ApiService.PostAsync<ServiceDTO>("Services/create", new {services = dto, token=AuthService.GetInstance().token});
        } catch (HttpRequestException ex) {
            error = true;
        }
        catch (Exception ex) { }

        if (error)
        {
           var box = MessageBoxManager.GetMessageBoxStandard("Info", "Erreur lors de l'ajout du site.", ButtonEnum.Ok);
           var result = await box.ShowAsync();
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Info", "Service ajouté avec succès.", ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }



    }


}
