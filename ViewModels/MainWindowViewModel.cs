using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Views;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;


namespace Bloc4_GUI.ViewModels;

public partial class MainWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, IRoutableViewModel> VisitSalarie  { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> VisitService  { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> VisitSite  { get; }
    public ReactiveCommand<Unit, Unit> OpenSecretCommand { get; }

    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel() {

        OpenSecretCommand = ReactiveCommand.Create(OpenSecretLogin);

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
}
