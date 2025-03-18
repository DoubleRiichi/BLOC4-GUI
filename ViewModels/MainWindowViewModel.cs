using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Services;
using Bloc4_GUI.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using Splat;
using System;
using System.Net.Http;
using System.Reactive;


namespace Bloc4_GUI.ViewModels;

public partial class MainWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();
    public bool Connected {
        get => AuthService.Connected;
    }

    public ReactiveCommand<Unit, IRoutableViewModel> VisitSalarie  { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> VisitService  { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> VisitSite  { get; }
    public ReactiveCommand<Unit, Unit> OpenSecretCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }


    public MainWindowViewModel() {

        Router.Navigate.Execute(new SalarieViewModel(this));

        OpenSecretCommand = ReactiveCommand.Create(OpenSecretLogin);
        LogoutCommand = ReactiveCommand.Create(Logout);

        VisitSalarie = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new SalarieViewModel(this))
        );
        
        VisitService = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new ServiceViewModel(this))
        );

        VisitSite = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new SiteViewModel(this))
        );
    }


    public async void OpenSecretLogin() {
       var window = new SecretLoginView{
            DataContext = new SecretLoginViewModel()
        };
        
        var mainWindow = Locator.Current.GetService<MainWindow>();

        // var modalViewModel = (AddSalarieView) window.DataContext;
        await window.ShowDialog(mainWindow);
        window.Close();
    }

    public async void Logout()
    {   
        try
        {
            await AuthService.Logout();
            var box = MessageBoxManager.GetMessageBoxStandard("Attention!", "Vous êtes à présent deconnecté.", ButtonEnum.Ok);
            var result = await box.ShowAsync();
        } catch (HttpRequestException ex)
        { }
        catch (Exception ex) { }
    }
}
