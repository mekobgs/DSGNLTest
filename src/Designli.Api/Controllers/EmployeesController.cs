using Designli.Application.DTOs;
using Designli.Application.DTOs;
using Designli.Application.Mapping;
using Designli.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Designli.Api.Controllers;

/// <summary>
/// Controller for managing employees.
/// </summary>
[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves all employees.
    /// </summary>
    /// <returns>A list of employees.</returns>
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_repository.GetAll());

    /// <summary>
    /// Retrieves a specific employee by ID.
    /// </summary>
    /// <param name="id">The employee ID.</param>
    /// <returns>The employee details.</returns>
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var employee = _repository.GetById(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="dto">The employee data.</param>
    /// <returns>The created employee.</returns>
    [HttpPost]
    public IActionResult Create(EmployeeDto dto)
    {
        var employee = dto.ToEntity();
        _repository.Add(employee);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="dto">The updated employee data.</param>
    /// <returns>No content.</returns>
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, EmployeeDto dto)
    {
        var existing = _repository.GetById(id);
        if (existing is null) return NotFound();

        var update = dto.ToEntity();
        update.Id = id;

        _repository.Update(update);
        return NoContent();
    }

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _repository.Delete(id);
        return NoContent();
    }
}