using Designli.Domain.Entities;

namespace Designli.Domain.Interfaces;

public interface IUserRepository
{
    IEnumerable<UserApp> GetAll();
    UserApp? GetByUsername(string username);
}
