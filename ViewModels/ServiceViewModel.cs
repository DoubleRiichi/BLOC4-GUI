using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Bloc4_GUI.Models;
using Bloc4_GUI.placeholder;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;


namespace Bloc4_GUI.ViewModels;

public class ServiceViewModel :  ReactiveObject, IRoutableViewModel {
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = "Services";

        public ObservableCollection<Service> Services { get; } = PlaceholderData.Services;
        public ServiceViewModel(IScreen screen) => HostScreen = screen;

}