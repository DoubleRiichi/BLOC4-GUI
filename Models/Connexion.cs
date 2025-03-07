namespace Bloc4_GUI.Models;


public class Connexion {
    public int? id {get; set;}
    public string? password {get; set;}

    public string? token {get; set;}

    public Connexion() {
        id = null;
        password = null;
        token = null;
    }
}