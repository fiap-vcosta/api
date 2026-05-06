using MediatR;
using Domain.Entities;

namespace Application.Cliente.Commands;

public class UpdateClienteCommand : IRequest<ClienteResponse>
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
