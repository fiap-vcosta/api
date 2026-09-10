using Api.Filters;
using Api.Presenters.Cliente;
using Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Cliente.Internal;

[ApiController]
[AllowAnonymous]
[ServiceKeyAuthorize]
[Route("api/internal/clientes")]
public class ClienteInternalController(
    IMediator mediator,
    ConsultarClientePorDocumentoPresenter presenter) : ControllerBase
{
    [HttpGet("por-documento/{documento}")]
    public async Task<IActionResult> PorDocumento(string documento, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(documento))
        {
            return BadRequest(new { Errors = new[] { "Documento é obrigatório." } });
        }

        var response = await mediator.Send(
            new ConsultarClientePorDocumentoQuery { Documento = documento },
            cancellationToken);

        return Ok(presenter.Present(response));
    }
}
