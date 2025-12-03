using BCrypt.Net;
using Designli.Domain.Entities;
using Designli.Domain.Interfaces;

namespace Designli.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public List<UserApp> Users { get; set; }

    public UserRepository()
    {
        Users = new()
        {
            new UserApp { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") },
            new UserApp { Username = "john", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") },
            new UserApp { Username = "sarah", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456") }
        };
    }

    public UserApp? GetByUsername(string username)
    {
        return Users.FirstOrDefault(x => x.Username == username);
    }
}
