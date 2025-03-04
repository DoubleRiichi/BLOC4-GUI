namespace Bloc4_GUI.Models;
using ReactiveUI;

public class Service : ModelBase{
    private int? _id;
    public int? id {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string? _nom;
    public string? nom {
        get => _nom;
        set => this.RaiseAndSetIfChanged(ref _nom, value);
    }

    private Site _site;
    public Site site {
        get => _site;
        set => this.RaiseAndSetIfChanged(ref _site, value);
    }

    private bool _hasChanges;
    public bool HasChanges {
        get => _hasChanges;
        set => this.RaiseAndSetIfChanged(ref _hasChanges, value);
    }

    public void MarkAsChanged() {
        HasChanges = true;
    }

    public Service Clone() {
        return (Service)this.MemberwiseClone();
    }
}