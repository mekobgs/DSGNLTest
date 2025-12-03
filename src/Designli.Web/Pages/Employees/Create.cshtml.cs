using Designli.Application.DTOs;
using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages.Employees;

public class CreateModel : PageModel
{
    private readonly EmployeeApiService _employeeService;

    [BindProperty]
    public EmployeeDto Input { get; set; } = new("", DateTime.Today, "", "");

    public string? ErrorMessage { get; set; }

    public CreateModel(EmployeeApiService employeeService)
    {
        _employeeService = employeeService;
    }

    public IActionResult OnGet()
    {
        // Check authentication
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var success = await _employeeService.CreateAsync(Input);

        if (success)
        {
            TempData["SuccessMessage"] = "Employee created successfully!";
            return RedirectToPage("Index");
        }

        ErrorMessage = "Failed to create employee. Please try again.";
        return Page();
    }
}
