using Api.Contracts.Validation;
using Api.Filters;
using Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Cliente.SystemApi;

[ApiController]
[AllowAnonymous]
[ServiceKeyAuthorize]
[Route("api/system/clientes")]
public class ClienteSystemController(IMediator mediator) : ControllerBase
{
    [HttpGet("por-documento/{documento}")]
    public async Task<IActionResult> PorDocumento(string documento, CancellationToken cancellationToken)
    {
        if (!DocumentoNormalizer.TryNormalizeValidCpfOrCnpj(documento, out var normalized, out var errors))
        {
            return BadRequest(new { Errors = errors });
        }

        var existe = await mediator.Send(
            new ConsultarClientePorDocumentoQuery { Documento = normalized },
            cancellationToken);

        return existe ? Ok() : NotFound();
    }
}
