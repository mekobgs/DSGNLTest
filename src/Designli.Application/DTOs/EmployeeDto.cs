namespace Designli.Application.DTOs;

public record EmployeeDto(
    string Name,
    DateTime BirthDate,
    string IdentityNumber,
    string Position
);
