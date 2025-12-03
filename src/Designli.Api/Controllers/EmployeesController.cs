using Designli.Application.DTOs;
using Designli.Application.Mapping;
using Designli.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Designli.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_repository.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var employee = _repository.GetById(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public IActionResult Create(EmployeeDto dto)
    {
        var employee = dto.ToEntity();
        _repository.Add(employee);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

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

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _repository.Delete(id);
        return NoContent();
    }
}