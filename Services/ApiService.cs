using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
namespace Bloc4_GUI.Services;


public static class ApiService {

    //todo: declare the api_url in a config file and let the program load it
    private readonly static string API_URL = "";
    
    private static readonly HttpClient _httpClient = new HttpClient{
        BaseAddress = new Uri(API_URL)
    };


    public static async Task<HttpStatusCode> Ping()
    {
        var response = await _httpClient.GetAsync("/");
        return response.StatusCode;

    }

    public static async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(jsonResponse);
    }

    public static async Task<T> PostAsync<T>(string endpoint, object payload)
    {
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonResponse);
    }

    public static async Task<bool> PingAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/test");
            response.EnsureSuccessStatusCode(); // Throw if HTTP response is an error
            Console.WriteLine("Code 200");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<T> PutAsync<T>(string endpoint, object payload)
    {
    
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonResponse);
    }
   
}