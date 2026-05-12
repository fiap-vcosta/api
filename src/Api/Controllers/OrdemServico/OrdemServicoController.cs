using Api.Contracts.Validation;
using Api.Controllers.OrdemServico.AdicionarItemServico;
using Api.Controllers.OrdemServico.AprovarServicosParcialmente;
using Api.Controllers.OrdemServico.ConfirmarExecucao;
using Api.Controllers.OrdemServico.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;
using Application.Core.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;
using Application.Core.OrdemServico.Commands.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.DescartarOrdemServico;
using Application.Core.OrdemServico.Commands.FinalizarDiagnostico;
using Application.Core.OrdemServico.Commands.RejeitarOrdemServico;
using Application.Core.OrdemServico.Queries.GetOrdemServicoById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.OrdemServico;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OrdemServicoController(
    IMediator mediator,
    IValidator<CriarOrdemServicoRequest> createOrdemServicoRequestValidator,
    IValidator<AdicionarItemServicoRequest> adicionarItemServicoRequestValidator,
    IValidator<AprovarServicosParcialmenteRequest> aprovarServicosParcialmenteRequestValidator,
    IValidator<ConfirmarExecucaoRequest> confirmarExecucaoRequestValidator
) : ControllerBase
{
    [HttpGet]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetOrdemServicoByIdQuery() { Id = id };
        var response = await mediator.Send(query);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> CriarOrdemServico([FromBody] CriarOrdemServicoRequest request)
    {
        var validationResult = createOrdemServicoRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }
        
        try
        {
            var command = new CriarOrdemServicoCommand { IdVeiculo = request.IdVeiculo };

            var response = await mediator.Send(command);
            return Created(nameof(GetById), response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/descartar")]
    public async Task<IActionResult> DescartarOrdemServico(int id)
    {
        try
        {
            var command = new DescartarOrdemServicoCommand() { IdOrdemServico = id };
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
    
    [HttpPost("{id:int}/adicionar-servico")]
    public async Task<IActionResult> AdicionarItemServico(int id, [FromBody] AdicionarItemServicoRequest request)
    {
        var validationResult = adicionarItemServicoRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }
        
        try
        {
            var command = new AdicionarItemOrdemServicoCommand
            {
                IdOrdemServico = id,
                IdServico = request.IdServico,
                ValorCobrado = request.ValorCobrado,
                ItensNecessarios = request.ItensNecessarios.Select(item =>
                    new AdicionarItemOrdemServicoCommand.ItemNecessario
                    {
                        IdItemEstoque = item.IdItemEstoque,
                        Quantidade = item.Quantidade
                    }).ToList()
            };

            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/finalizar-diagnostico")]
    public async Task<IActionResult> AdicionarItemServico(int id)
    {
        try
        {
            var command = new FinalizarDiagnosticoCommand() { IdOrdemServico = id };
            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/rejeitar")]
    public async Task<IActionResult> RejeitarOrdemServico(int id)
    {
        try
        {
            var command = new RejeitarOrdemServicoCommand() { IdOrdemServico = id };
            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/aprovar")]
    public async Task<IActionResult> AprovarOdemServico(int id)
    {
        try
        {
            var command = new AprovarOrdemServicoCommand() { IdOrdemServico = id };
            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/aprovar-parcialmente")]
    public async Task<IActionResult> AprovarServicosParcialmente(int id, [FromBody] AprovarServicosParcialmenteRequest request)
    {
        var validationResult = aprovarServicosParcialmenteRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }
        
        try
        {
            var command = new AprovarServicosParcialmenteCommand()
            {
                IdOrdemServico = id,
                IdServicosAprovados = request.IdsServicosAprovados
            };
            
            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("{id:int}/confirmar-execucao")]
    public async Task<IActionResult> ConfirmarExecucao(int id, [FromBody] ConfirmarExecucaoRequest request)
    {
        var validationResult = confirmarExecucaoRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { validationResult.Errors });
        }
        
        try
        {
            var command = new ConfirmarExecucaoOrdemServicoCommand()
            {
                IdOrdemServico = id,
                ServicoExecutados = request.ServicosExecutados
            };
            
            var response = await mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}