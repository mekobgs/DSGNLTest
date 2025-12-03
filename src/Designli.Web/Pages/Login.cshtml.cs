using Designli.Application.DTOs;
using Designli.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class LoginModel : PageModel
{
    private readonly AuthApiService _authApiService;

    public LoginModel(AuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {      
        if (string.IsNullOrEmpty(Input.Username) || string.IsNullOrEmpty(Input.Password))
        {
            ErrorMessage = "Username and password are required";
            return Page();
        }

        var token = await _authApiService.LoginAsync(Input.Username, Input.Password);
        
        if (token == null)
        {
            ErrorMessage = "Invalid login";
            return Page();
        }

        // Store JWT token in session
        HttpContext.Session.SetString("jwt_token", token);

        return RedirectToPage("/Index");
    }
}
