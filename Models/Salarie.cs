namespace Bloc4_GUI.Models;

using ReactiveUI;

public class Salarie : ModelBase {
    // Properties that trigger change notifications when modified
    private int? _id;
    public int? id {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string _nom;
    public string nom {
        get => _nom;
        set => this.RaiseAndSetIfChanged(ref _nom, value);
    }

    private string _prenom;
    public string prenom {
        get => _prenom;
        set => this.RaiseAndSetIfChanged(ref _prenom, value);
    }

    private string _telephone_fixe;
    public string telephone_fixe {
        get => _telephone_fixe;
        set => this.RaiseAndSetIfChanged(ref _telephone_fixe, value);
    }

    private string _telephone_mobile;
    public string telephone_mobile {
        get => _telephone_mobile;
        set => this.RaiseAndSetIfChanged(ref _telephone_mobile, value);
    }

    private string _email;
    public string email {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    private Service _service;
    public Service service {
        get => _service;
        set => this.RaiseAndSetIfChanged(ref _service, value);
    }

    // Permet de connaître lorsqu'un objet Salarié est modifié dans la datagrid
    private bool _hasChanges;
    public bool HasChanges {
        get => _hasChanges;
        set => this.RaiseAndSetIfChanged(ref _hasChanges, value);
    }

    public void MarkAsChanged() {
        HasChanges = true;
    }

    // Clone method to duplicate the object (excluding the INotifyPropertyChanged behavior)
    public Salarie Clone() {
        return (Salarie)this.MemberwiseClone();
    }
}
