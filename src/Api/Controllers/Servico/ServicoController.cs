using Api.Contracts.Validation;
using Api.Controllers.Servico.CreateServico;
using Api.Controllers.Servico.UpdateServico;
using Application.Administrativo.Servico.Commands;
using Application.Administrativo.Servico.Commands.CreateServico;
using Application.Administrativo.Servico.Commands.DeleteServico;
using Application.Administrativo.Servico.Commands.UpdateServico;
using Application.Administrativo.Servico.Queries;
using Application.Administrativo.Servico.Queries.GetAllServicos;
using Application.Administrativo.Servico.Queries.GetServicoById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Servico;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ServicoController(
    IMediator mediator,
    IValidator<CreateServicoRequest> createValidator,
    IValidator<UpdateServicoRequest> updateValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServicoRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new CreateServicoCommand
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                PrecoPadrao = request.PrecoPadrao,
                Ativo = request.Ativo
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
        var query = new GetServicoByIdQuery { Id = id };
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
        var query = new GetAllServicosQuery();
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServicoRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }

        try
        {
            var command = new UpdateServicoCommand
            {
                Id = id,
                Codigo = request.Codigo,
                Nome = request.Nome,
                PrecoPadrao = request.PrecoPadrao,
                Ativo = request.Ativo
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
            var command = new DeleteServicoCommand { Id = id };
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
