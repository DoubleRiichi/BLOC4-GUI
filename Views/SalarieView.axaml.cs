using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.ViewModels;
using ReactiveUI;

namespace Bloc4_GUI.Views;

public partial class SalarieView : ReactiveUserControl<SalarieViewModel> {
    public SalarieView() {
        this.WhenActivated(disposables => {});
        AvaloniaXamlLoader.Load(this);

        var dataGrid = this.FindControl<DataGrid>("SalariesGrid");
        dataGrid.CellEditEnded += (sender, e) => {
            ViewModel.HandleCellEdit(e);
        };

        dataGrid.PreparingCellForEdit += (sender, e) => {
            ViewModel.SaveCellState(e);
        };

    }


    private void ChangePage(object sender, SelectionChangedEventArgs e) {
        var comboBox = sender as ComboBox;
        var selectedItem = comboBox?.SelectedItem as string;
        if (selectedItem != null) {
            ViewModel.GoToPage(int.Parse(selectedItem));
        }
    }

}