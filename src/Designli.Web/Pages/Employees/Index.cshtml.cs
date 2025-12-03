using Designli.Web.Services;
using Designli.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages.Employees;

public class IndexModel : PageModel
{
    private readonly EmployeeApiService _employeeService;

    public List<Employee> Employees { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;

    public IndexModel(EmployeeApiService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        // Check authentication
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        SearchTerm = search ?? string.Empty;
        var allEmployees = await _employeeService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            Employees = allEmployees
                .Where(e => e.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                           e.IdentityNumber.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                           e.Position.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        else
        {
            Employees = allEmployees;
        }

        return Page();
    }
}
