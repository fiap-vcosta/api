using Api.Contracts.Validation;
using Api.Controllers.Cliente.CreateCliente;
using Api.Controllers.Cliente.UpdateCliente;
using Application.Administrativo.Cliente.Commands;
using Application.Administrativo.Cliente.Queries;
using Domain.Administrativo.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Cliente;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ClienteController(
    IMediator mediator,
    IValidator<CreateClienteRequest> createValidator,
    IValidator<UpdateClienteRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new CreateClienteCommand
            {
                Nome = request.Nome,
                TipoDocumento = (TipoDocumento)request.TipoDocumento,
                Documento = request.Documento
            };

            var response = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetClienteByIdQuery { Id = id };
        var response = await mediator.Send(query);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllClientesQuery();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new UpdateClienteCommand
            {
                Id = id,
                Nome = request.Nome,
                TipoDocumento = (TipoDocumento)request.TipoDocumento,
                Documento = request.Documento
            };

            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var command = new DeleteClienteCommand { Id = id };
            await mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
