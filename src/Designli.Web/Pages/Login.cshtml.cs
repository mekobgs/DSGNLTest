using Designli.Application.DTOs;
using Designli.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public LoginModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnPost()
    {      
        if (string.IsNullOrEmpty(Input.Username) || string.IsNullOrEmpty(Input.Password))
        {
            ErrorMessage = "Username and password are required";
            return Page();
        }

        var user = _userRepository.GetByUsername(Input.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
        {
            ErrorMessage = "Invalid login";
            return Page();
        }

        // Store username in session (simplified authentication)
        HttpContext.Session.SetString("username", user.Username);

        return RedirectToPage("/Users");
    }
}
