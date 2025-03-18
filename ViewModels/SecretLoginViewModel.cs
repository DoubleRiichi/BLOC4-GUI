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
using Bloc4_GUI.Views;
using System.Net.Http;

namespace Bloc4_GUI.ViewModels;


public class SecretLoginViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public bool Connected
    {
        get => AuthService.Connected;
    }
    public bool NotConnected
    {
        get => !AuthService.Connected;
    }

    private string? _inputPassword;
    
    public string? InputPassword {
        get => _inputPassword;
        set => this.RaiseAndSetIfChanged(ref _inputPassword, value);
    }
    public SecretLoginViewModel()
    {
        LoginCommand = ReactiveCommand.Create(Login);
        LogoutCommand = ReactiveCommand.Create(Logout);
    }


    public async void InitializeAsync() {
    
    }


    public async void Login() {
        var result = await AuthService.Login(1, _inputPassword);

        if (result) {
            var box = MessageBoxManager.GetMessageBoxStandard("!", "Vous êtes à présent connecté!", ButtonEnum.Ok);
            await box.ShowAsync();            
        } else {
            var box = MessageBoxManager.GetMessageBoxStandard("!", "La connexion n'a pas pu aboutir.", ButtonEnum.Ok);
            await box.ShowAsync();   
        }
    }

    public async void Logout()
    {
        try
        {
            await AuthService.Logout();
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes à présent deconnecté.", ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }
        catch (HttpRequestException ex)
        {

        }
        catch (Exception ex) { }
    }
}
