using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;   
using Bloc4_GUI.ViewModels;
using Bloc4_GUI.Views;
namespace Bloc4_GUI;

public class AppViewLocator : ReactiveUI.IViewLocator
    {
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            SalarieViewModel context => new SalarieView { DataContext = context },
            ServiceViewModel context => new ServiceView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
}