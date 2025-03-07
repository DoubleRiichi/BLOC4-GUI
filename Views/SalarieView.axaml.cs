using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class SalarieView : ReactiveUserControl<SalarieViewModel> {
    public SalarieView() {
        this.WhenActivated(disposables => {

            _ = ViewModel.InitializeAsync();
        });


        AvaloniaXamlLoader.Load(this);
        

        var dataGrid = this.FindControl<DataGrid>("SalariesGrid");
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
                var salarie = button.DataContext as Salarie;

                if (salarie != null) {
                    ViewModel.DeleteSalarie(salarie);
                }
            }
    }



}