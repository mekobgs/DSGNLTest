using Designli.Application.DTOs;
using Designli.Domain.Entities;
using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages.Employees;

public class EditModel : PageModel
{
    private readonly EmployeeApiService _employeeService;

    [BindProperty]
    public EmployeeDto Input { get; set; } = new("", DateTime.Today, "", "");

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? ErrorMessage { get; set; }

    public EditModel(EmployeeApiService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check authentication
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        var employee = await _employeeService.GetByIdAsync(Id);
        if (employee == null)
        {
            return RedirectToPage("Index");
        }

        Input = new EmployeeDto(
            employee.Name,
            employee.BirthDate,
            employee.IdentityNumber,
            employee.Position
        );

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var success = await _employeeService.UpdateAsync(Id, Input);

        if (success)
        {
            TempData["SuccessMessage"] = "Employee updated successfully!";
            return RedirectToPage("Index");
        }

        ErrorMessage = "Failed to update employee. Please try again.";
        return Page();
    }
}
