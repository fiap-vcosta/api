using Application.UseCases.Administrativo.Servico.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.CreateServico;

public class CreateServicoCommand : IRequest<ServicoResponse>
{
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoPadrao { get; init; }
    public bool Ativo { get; init; }
}
