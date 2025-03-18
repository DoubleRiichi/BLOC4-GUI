using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Bloc4_GUI.Models;

namespace Bloc4_GUI.Services;




public sealed class AuthService {
    public static bool Connected {get; set;} = false;
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
            Connected = true;
            return true;
        } catch (Exception ex) {
            return false;
        }
    }


    public static async Task<bool> Logout() {
        var instance = GetInstance();
        Connected = false;

        if (instance.id != null && instance.token != null) {
            try {
                await ApiService.PostAsync<Connexion>("Connexion/logout", instance);
                currentConnexion = null;
                return true;
            } catch (HttpRequestException ex) {
                
                return false;
            }
            catch (Exception ex) { }
        }        
        return false;
    }

}