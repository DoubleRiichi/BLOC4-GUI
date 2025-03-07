
using Bloc4_GUI.Models;

namespace Bloc4_GUI.DTO;

public class SalarieDTO {

    public int? id {get; set;}
    public string nom {get; set;}
    public string prenom {get; set;}
    public string telephone_fixe {get; set;}
    public string telephone_mobile {get; set;}
    public string email {get; set;}
    public int? services_id {get; set;}

    public SalarieDTO(Salarie salarie) {
        id = salarie.id;
        nom = salarie.nom;
        prenom = salarie.prenom;
        telephone_fixe = salarie.telephone_fixe;
        telephone_mobile = salarie.telephone_mobile;
        email = salarie.email;
        services_id = salarie.service.id;
    }
}