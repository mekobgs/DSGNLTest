using Designli.Domain.Entities;

namespace Designli.Domain.Interfaces;

public interface IEmployeeRepository
{
    IEnumerable<Employee> GetAll();
    Employee? GetById(Guid id);
    void Add(Employee employee);
    void Update(Employee employee);
    void Delete(Guid id);
}
