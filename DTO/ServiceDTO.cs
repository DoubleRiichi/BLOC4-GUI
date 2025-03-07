

using Bloc4_GUI.Models;

namespace Bloc4_GUI.DTO;


public class ServiceDTO {
    public int? id { get; set; }
    public string nom { get; set; }
    public int? sites_id { get; set; }


    public ServiceDTO(Service service) {
        this.id = service.id;
        this.nom = service.nom;
        this.sites_id = service.site.id;
    }
}