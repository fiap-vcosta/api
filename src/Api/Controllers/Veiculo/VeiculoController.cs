using Api.Contracts;
using Application.Veiculo.Commands;
using Application.Veiculo.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Veiculo;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VeiculoController(
    IMediator mediator,
    IValidator<CreateVeiculoRequest> createValidator,
    IValidator<UpdateVeiculoRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVeiculoRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new CreateVeiculoCommand
            {
                Placa = request.Placa,
                DonoId = request.DonoId,
                Modelo = request.Modelo,
                Marca = request.Marca
            };

            var response = await mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetVeiculoByIdQuery { Id = id };
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
        var query = new GetAllVeiculosQuery();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("por-dono/{donoId}")]
    public async Task<IActionResult> GetByDono(int donoId)
    {
        var query = new GetVeiculosByDonoQuery { DonoId = donoId };
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVeiculoRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new UpdateVeiculoCommand
            {
                Id = id,
                Placa = request.Placa,
                DonoId = request.DonoId,
                Modelo = request.Modelo,
                Marca = request.Marca
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var command = new DeleteVeiculoCommand { Id = id };
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
