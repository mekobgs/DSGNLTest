using Designli.Domain.Entities;

namespace Designli.Domain.Interfaces;

public interface IUserRepository
{
    List<UserApp> Users { get; set; }
    UserApp? GetByUsername(string username);
}
