namespace Designli.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public DateTime BirthDate { get; set; } 
    public string IdentityNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}