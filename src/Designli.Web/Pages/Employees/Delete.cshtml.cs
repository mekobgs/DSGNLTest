using Designli.Domain.Entities;
using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages.Employees;

public class DeleteModel : PageModel
{
    private readonly EmployeeApiService _employeeService;

    public Employee? Employee { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public DeleteModel(EmployeeApiService employeeService)
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

        Employee = await _employeeService.GetByIdAsync(Id);
        if (Employee == null)
        {
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var success = await _employeeService.DeleteAsync(Id);

        if (success)
        {
            TempData["SuccessMessage"] = "Employee deleted successfully!";
        }

        return RedirectToPage("Index");
    }
}
