using BCrypt.Net;
using Designli.Domain.Entities;
using Designli.Domain.Interfaces;

namespace Designli.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<UserApp> _users;
    private readonly object _lock = new();

    public UserRepository()
    {
        _users = new()
        {
            new UserApp { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") },
            new UserApp { Username = "john", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") },
            new UserApp { Username = "sarah", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456") }
        };
    }

    public IEnumerable<UserApp> GetAll()
    {
        lock (_lock)
        {
            return _users.ToList();
        }
    }

    public UserApp? GetByUsername(string username)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(x => x.Username == username);
        }
    }
}
