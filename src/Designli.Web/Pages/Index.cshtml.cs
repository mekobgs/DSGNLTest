using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class IndexModel : PageModel
{
    private readonly EmployeeApiService _employeeService;
    private readonly AuthApiService _authService;

    public int TotalEmployees { get; set; }
    public int TotalUsers { get; set; }

    public IndexModel(EmployeeApiService employeeService, AuthApiService authService)
    {
        _employeeService = employeeService;
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check authentication
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        // Load statistics
        var employees = await _employeeService.GetAllAsync();
        var users = await _authService.GetUsersAsync();

        TotalEmployees = employees.Count;
        TotalUsers = users.Count;

        return Page();
    }
}
