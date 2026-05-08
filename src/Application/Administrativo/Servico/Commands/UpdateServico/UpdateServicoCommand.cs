using Application.Administrativo.Servico.Commands.CreateServico;
using MediatR;

namespace Application.Administrativo.Servico.Commands.UpdateServico;

public class UpdateServicoCommand : IRequest<ServicoResponse>
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoPadrao { get; init; }
    public bool Ativo { get; init; }
}
