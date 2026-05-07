using Domain.Administrativo.Entities;
using MediatR;

namespace Application.Administrativo.Cliente.Commands;

public class CreateClienteCommand : IRequest<ClienteResponse>
{
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}

public class ClienteResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public int TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
