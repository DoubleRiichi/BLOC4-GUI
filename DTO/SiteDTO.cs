

using Bloc4_GUI.Models;

namespace Bloc4_GUI.DTO;


public class SiteDTO {
    public int? id { get; set; }
    public string nom { get; set; }


    public SiteDTO(Site site) {
        this.id = site.id;
        this.nom = site.nom;
    }
}