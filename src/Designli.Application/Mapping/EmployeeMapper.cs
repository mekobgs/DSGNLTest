using Designli.Application.DTOs;
using Designli.Domain.Entities;

namespace Designli.Application.Mapping;

public static class EmployeeMapper
{
    public static Employee ToEntity(this EmployeeDto dto)
    {
        return new Employee
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            IdentityNumber = dto.IdentityNumber,
            Position = dto.Position
        };
    }
}
