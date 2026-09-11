using MediatR;

namespace Application.UseCases.OrdemServico.Queries.GetOrdemServicoByTokenEDocumento;

public class GetOrdemServicoByTokenEDocumentoQuery : IRequest<int>
{
    public required string TokenAprovacao { get; init; }
    public required string DocumentoCliente { get; init; }
}
