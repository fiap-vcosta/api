using Api.Contracts.Validation;
using Api.Controllers.OrdemServico.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.CriarOrdemServico;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.OrdemServico;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OrdemServicoController(
    IMediator mediator,
    IValidator<CriarOrdemServicoRequest> createOrdemServicoRequestValidator
) : ControllerBase
{
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
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}