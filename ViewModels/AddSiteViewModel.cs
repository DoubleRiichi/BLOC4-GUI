using Bloc4_GUI.placeholder;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Bloc4_GUI.placeholder;
using Bloc4_GUI.Models;
using Bloc4_GUI.Services;
using DynamicData;
using System.Collections.Generic;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;
using Bloc4_GUI.DTO;
using System;

namespace Bloc4_GUI.ViewModels;


public class AddSiteViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> CreateSiteCommand { get; }

    private string? _inputNom;
    
    public string? InputSite {
        get => _inputNom;
        set => this.RaiseAndSetIfChanged(ref _inputNom, value);
    }
    public AddSiteViewModel()
    {
        CreateSiteCommand = ReactiveCommand.Create(CreateSite);
    }


    public async void InitializeAsync() {
    }


    public async Task<bool> Validate() {
        string message = "";
        bool error = false;

        if(!ValidatorService.isValidName(InputSite)) {
            message += $"{InputSite} n'est pas un nom valide pour un service!\n";
            error = true;
        }

        if (error) {
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", message, ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return false;
        }

        return true;
    }  

    public async void CreateSite() {
        bool validSite = await Validate();

        if (!validSite) {
            return;
        }

        var dto = new SiteDTO(
            new Site {
            id = null,
            nom = InputSite,
        });

        try {
            await ApiService.PostAsync<SiteDTO>("Sites/create", new {site = dto, token = AuthService.GetInstance().token});
        } catch (Exception ex) {
            
        }
    }  
}