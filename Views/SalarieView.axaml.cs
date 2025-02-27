using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class SalarieView : ReactiveUserControl<SalarieViewModel> {
    public SalarieView() {
        this.WhenActivated(disposables => {});
        AvaloniaXamlLoader.Load(this);
    }
}