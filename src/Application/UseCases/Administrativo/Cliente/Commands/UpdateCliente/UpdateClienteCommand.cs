using Application.UseCases.Administrativo.Cliente.Responses;
using Domain.Administrativo.Entities;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Commands.UpdateCliente;

public class UpdateClienteCommand : IRequest<ClienteResponse>
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
