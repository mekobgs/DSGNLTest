namespace Designli.Web.Services;

public interface IAuthApiService
{
    Task<string?> LoginAsync(string username, string password);
    Task<List<string>> GetUsersAsync();
}
