using Api.Presenters.OrdemServico;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.PublicApi.OrdemServico;

[ApiController]
[AllowAnonymous]
[Route("api/public/ordens-servico")]
public class OrdemServicoPublicController(IMediator mediator, OrdemServicoPresenter presenter) : ControllerBase
{
    private static readonly string[] TokenObrigatorioErrors = ["Token é obrigatório."];

    [HttpPost("aprovar")]
    public async Task<IActionResult> Aprovar([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { Errors = TokenObrigatorioErrors });
        }

        var response = await mediator.Send(new AprovarOrdemServicoPorTokenCommand { TokenAprovacao = token });
        return Ok(presenter.Present(response));
    }

    [HttpPost("rejeitar")]
    public async Task<IActionResult> Rejeitar([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { Errors = TokenObrigatorioErrors });
        }

        var response = await mediator.Send(new RejeitarOrdemServicoPorTokenCommand { TokenAprovacao = token });
        return Ok(presenter.Present(response));
    }
}
