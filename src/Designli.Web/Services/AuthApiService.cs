using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Designli.Web.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        try 
        {
            Console.WriteLine($"[AuthApiService] Attempting login for user: {username} to {_httpClient.BaseAddress}api/auth/login");
            var loginRequest = new { Username = username, Password = password };
            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/auth/login", content);
            
            Console.WriteLine($"[AuthApiService] Login response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[AuthApiService] Login failed. Content: {errorContent}");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(responseContent);
            
            return result?.Token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthApiService] Login exception: {ex.Message}");
            Console.WriteLine($"[AuthApiService] Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<List<string>> GetUsersAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt_token");
        
        if (string.IsNullOrEmpty(token))
            return new List<string>();

        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/auth/users");
        
        if (!response.IsSuccessStatusCode)
            return new List<string>();

        var content = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<string>>(content);
        
        return users ?? new List<string>();
    }

    private class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
