using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class ServiceView : ReactiveUserControl<ServiceViewModel> {
    public ServiceView() {
        this.WhenActivated(disposables => {});
        AvaloniaXamlLoader.Load(this);

        var dataGrid = this.FindControl<DataGrid>("ServicesGrid");
        dataGrid.CellEditEnded += (sender, e) => {
            ViewModel.HandleCellEdit(e);
        };

        dataGrid.PreparingCellForEdit += (sender, e) => {
            ViewModel.SaveCellState(e);
        };

    }

}