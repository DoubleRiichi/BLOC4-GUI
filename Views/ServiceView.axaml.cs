using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class ServiceView : ReactiveUserControl<ServiceViewModel> {
    public ServiceView() {
        this.WhenActivated(disposables => {
            _ =  ViewModel.InitializeAsync();

        });
        AvaloniaXamlLoader.Load(this);

        var dataGrid = this.FindControl<DataGrid>("ServicesGrid");
        dataGrid.CellEditEnded += (sender, e) => {
            ViewModel.HandleCellEdit(e);
        };

        dataGrid.PreparingCellForEdit += (sender, e) => {
            ViewModel.SaveCellState(e);
        };

    }


       private void DeleteButton_Click(object sender, RoutedEventArgs e) {
        var button = sender as Button;
            if (button != null) {
                var service = button.DataContext as Service;

                if (service != null) {
                    ViewModel.DeleteService(service);
                }
            }
    }

}