
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class AddServiceView : ReactiveWindow<AddServiceViewModel> {
    public AddServiceView() {
        this.WhenActivated(disposables => {
            ViewModel.InitializeAsync();
        });
        AvaloniaXamlLoader.Load(this);
        this.CanResize = false;
    
    }
}