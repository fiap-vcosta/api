using Api.Contracts.Validation;
using Api.Controllers.Veiculo.CreateVeiculo;
using Api.Controllers.Veiculo.UpdateVeiculo;
using Api.Presenters.Veiculo;
using Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;
using Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;
using Application.UseCases.Administrativo.Veiculo.Commands.UpdateVeiculo;
using Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;
using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;
using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Veiculo;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VeiculoController(
    IMediator mediator,
    VeiculoPresenter presenter,
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

        var command = new CreateVeiculoCommand
        {
            Placa = request.Placa,
            IdCliente = request.IdCliente,
            Modelo = request.Modelo,
            Marca = request.Marca
        };

        var response = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, presenter.Present(response));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetVeiculoByIdQuery { Id = id };
        var response = await mediator.Send(query);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(presenter.Present(response));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllVeiculosQuery();
        var response = await mediator.Send(query);
        return Ok(presenter.Present(response));
    }

    [HttpGet("por-dono/{clienteId:int}")]
    public async Task<IActionResult> GetByDono(int clienteId)
    {
        var query = new GetVeiculosByClienteQuery { IdCliente = clienteId };
        var response = await mediator.Send(query);
        return Ok(presenter.Present(response));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVeiculoRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        var command = new UpdateVeiculoCommand
        {
            Id = id,
            Placa = request.Placa,
            IdCliente = request.IdCliente,
            Modelo = request.Modelo,
            Marca = request.Marca
        };

        var response = await mediator.Send(command);
        return Ok(presenter.Present(response));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteVeiculoCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}
