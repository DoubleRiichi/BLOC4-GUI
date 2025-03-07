using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class SiteView : ReactiveUserControl<SiteViewModel> {
    public SiteView() {
        this.WhenActivated(disposables => {
            _ =  ViewModel.InitializeAsync();

        });

        AvaloniaXamlLoader.Load(this);

        var dataGrid = this.FindControl<DataGrid>("SitesGrid");
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
                var site = button.DataContext as Site;

                if (site != null) {
                    ViewModel.DeleteSite(site);
                }
            }
    }

}