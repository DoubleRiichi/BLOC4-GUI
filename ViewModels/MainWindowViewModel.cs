using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;


namespace Bloc4_GUI.ViewModels;

public partial class MainWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, IRoutableViewModel> VisitSalarie  { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> VisitService  { get; }

    public string Greeting { get; } = "Welcome to Avalonia!";

    public MainWindowViewModel() {
        VisitSalarie = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new SalarieViewModel(this))
        );
        
        VisitService = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new ServiceViewModel(this))
        );
    }
}
