using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Designli.Application.DTOs;
using Designli.Domain.Entities;

namespace Designli.Web.Services;

public class EmployeeApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeeApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/employees");
            
            if (!response.IsSuccessStatusCode)
                return new List<Employee>();

            var content = await response.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<Employee>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return employees ?? new List<Employee>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmployeeApiService] GetAll exception: {ex.Message}");
            return new List<Employee>();
        }
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/employees/{id}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var employee = JsonSerializer.Deserialize<Employee>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return employee;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmployeeApiService] GetById exception: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CreateAsync(EmployeeDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/employees", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmployeeApiService] Create exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, EmployeeDto dto)
    {
        try
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/employees/{id}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmployeeApiService] Update exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/employees/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmployeeApiService] Delete exception: {ex.Message}");
            return false;
        }
    }
}
