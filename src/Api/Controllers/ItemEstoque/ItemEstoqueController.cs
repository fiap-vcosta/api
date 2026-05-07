using Api.Contracts;
using Application.ItemEstoque.Commands;
using Application.ItemEstoque.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ItemEstoque;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ItemEstoqueController(
    IMediator mediator,
    IValidator<CreateItemEstoqueRequest> createValidator,
    IValidator<UpdateItemEstoqueRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemEstoqueRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new CreateItemEstoqueCommand
            {
                Codigo = request.Codigo,
                Tipo = request.Tipo,
                Nome = request.Nome,
                UnidadeMedida = request.UnidadeMedida,
                PrecoVenda = request.PrecoVenda,
                Saldo = request.Saldo,
                SaldoReservado = request.SaldoReservado
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
        var query = new GetItemEstoqueByIdQuery { Id = id };
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
        var query = new GetAllItemEstoqueQuery();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemEstoqueRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new UpdateItemEstoqueCommand
            {
                Id = id,
                Codigo = request.Codigo,
                Tipo = request.Tipo,
                Nome = request.Nome,
                UnidadeMedida = request.UnidadeMedida,
                PrecoVenda = request.PrecoVenda,
                Saldo = request.Saldo,
                SaldoReservado = request.SaldoReservado
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
            var command = new DeleteItemEstoqueCommand { Id = id };
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
