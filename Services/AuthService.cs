using System;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Bloc4_GUI.Models;

namespace Bloc4_GUI.Services;




public sealed class AuthService {
    bool Connected {get; set;} = false;
    private static Connexion? currentConnexion {get; set;}


    public static Connexion GetInstance() {
        if (currentConnexion == null) {
            currentConnexion = new Connexion();
        }

        return currentConnexion;
    }


    public static async Task<bool> Login(int id, string password) {
        GetInstance().id = id;
        GetInstance().password = password;
        
        try {
            var response = await ApiService.PostAsync<Connexion>("Connexion/login", GetInstance());
            GetInstance().token = response.token;
            GetInstance().password = "";
            return true;
        } catch (Exception ex) {
            return false;
        }
    }


    public static async Task<bool> Logout() {
        var instance = GetInstance();

        if(instance.id != null && instance.token != null) {
            try {
                await ApiService.PostAsync<Connexion>("Connexion/logout", instance);
                currentConnexion = null;
                return true;
            } catch (Exception ex) {
                
                return false;
            }
        }        
        
        return false;
    }

}