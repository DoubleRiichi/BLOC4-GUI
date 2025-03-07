using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Bloc4_GUI.ViewModels;
using ReactiveUI;


namespace Bloc4_GUI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);

        // var keyBinding = new KeyBinding
        // {
        //     Command = ((MainWindowViewModel)DataContext).OpenSecretCommand,  // Bind to your ReactiveCommand
        //     Gesture = new KeyGesture(Key.X, KeyModifiers.Control)  // Ctrl+X
        // };
        
        // // Attach the KeyBinding to the InputBindings of the Window
        // this.InputBindings.Add(keyBinding);
        // 
        }
    }