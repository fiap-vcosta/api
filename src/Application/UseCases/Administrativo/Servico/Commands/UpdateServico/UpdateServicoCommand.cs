using Application.UseCases.Administrativo.Servico.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.UpdateServico;

public class UpdateServicoCommand : IRequest<ServicoResponse>
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoPadrao { get; init; }
    public bool Ativo { get; init; }
}
