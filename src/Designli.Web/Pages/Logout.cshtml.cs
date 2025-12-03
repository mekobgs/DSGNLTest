using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Clear the session
        HttpContext.Session.Clear();

        // Redirect to login page
        return RedirectToPage("/Login");
    }
}
