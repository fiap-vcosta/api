using Api.Contracts.Validation;
using Api.Controllers.Cliente.CreateCliente;
using Api.Controllers.Cliente.UpdateCliente;
using Api.Presenters.Cliente;
using Api.Presenters.Veiculo;
using Application.UseCases.Administrativo.Cliente.Commands.CreateCliente;
using Application.UseCases.Administrativo.Cliente.Commands.DeleteCliente;
using Application.UseCases.Administrativo.Cliente.Commands.UpdateCliente;
using Application.UseCases.Administrativo.Cliente.Queries.GetAllClientes;
using Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;
using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Cliente;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ClienteController(
    IMediator mediator,
    ClientePresenter presenter,
    VeiculoPresenter veiculoPresenter,
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

        var command = new CreateClienteCommand
        {
            Nome = request.Nome,
            TipoDocumento = request.TipoDocumento,
            Documento = request.Documento
        };

        var response = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, presenter.Present(response));
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

        return Ok(presenter.Present(response));
    }

    [HttpGet("{id:int}/veiculos")]
    public async Task<IActionResult> GetVeiculos(int id)
    {
        var query = new GetVeiculosByClienteQuery { IdCliente = id };
        var response = await mediator.Send(query);
        return Ok(veiculoPresenter.Present(response));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllClientesQuery();
        var response = await mediator.Send(query);
        return Ok(presenter.Present(response));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        var command = new UpdateClienteCommand
        {
            Id = id,
            Nome = request.Nome,
            TipoDocumento = request.TipoDocumento,
            Documento = request.Documento
        };

        var response = await mediator.Send(command);
        return Ok(presenter.Present(response));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteClienteCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}
