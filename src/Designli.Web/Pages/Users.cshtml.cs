using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class UsersModel : PageModel
{
    private readonly AuthApiService _authApiService;

    public List<string> Users { get; set; } = new();

    public UsersModel(AuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated (has JWT token)
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        // Call JWT-protected API endpoint
        Users = await _authApiService.GetUsersAsync();
        
        return Page();
    }
}
