using Designli.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Designli.Web.Pages;

public class UsersModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public List<string> Users { get; set; } = new();

    public UsersModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void OnGet()
    {
        // Get all usernames directly from repository
        Users = _userRepository.Users.Select(u => u.Username).ToList();
    }
}
