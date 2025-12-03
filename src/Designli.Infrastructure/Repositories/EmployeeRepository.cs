using Designli.Domain.Entities;
using Designli.Domain.Interfaces;

namespace Designli.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees;
    private readonly object _lock = new();

    public EmployeeRepository()
    {
        _employees = new()
        {
            new Employee { Name = "John Doe", BirthDate = new DateTime(1990,1,5), IdentityNumber = "A12345", Position = "Developer" },
            new Employee { Name = "Sarah Lee", BirthDate = new DateTime(1985,4,12), IdentityNumber = "B98765", Position = "Designer" },
            new Employee { Name = "Luis Pérez", BirthDate = new DateTime(1993,6,9), IdentityNumber = "C11223", Position = "QA Engineer" }
        };
    }

    public IEnumerable<Employee> GetAll()
    {
        lock (_lock)
        {
            return _employees.ToList(); // Return copy to avoid enumeration issues
        }
    }

    public Employee? GetById(Guid id)
    {
        lock (_lock)
        {
            return _employees.FirstOrDefault(x => x.Id == id);
        }
    }

    public void Add(Employee employee)
    {
        lock (_lock)
        {
            _employees.Add(employee);
        }
    }

    public void Update(Employee employee)
    {
        lock (_lock)
        {
            var existing = _employees.FirstOrDefault(x => x.Id == employee.Id);
            if (existing == null) return;

            existing.Name = employee.Name;
            existing.BirthDate = employee.BirthDate;
            existing.IdentityNumber = employee.IdentityNumber;
            existing.Position = employee.Position;
        }
    }

    public void Delete(Guid id)
    {
        lock (_lock)
        {
            var e = _employees.FirstOrDefault(x => x.Id == id);
            if (e != null) _employees.Remove(e);
        }
    }
}