namespace Bloc4_GUI.Models;

public class Salarie : ModelBase {
    public int? id {get; set;}
    public required string nom {get; set;}
    public required string prenom { get; set; }
    public required string telephone_fixe { get; set; }
    public required string telephone_mobile { get; set; }
    public required string email { get; set; }
    public required Service service { get; set; }

}