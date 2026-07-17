using Application.UseCases.Administrativo.Cliente.Responses;
using Domain.Administrativo.Entities;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Commands.CreateCliente;

public class CreateClienteCommand : IRequest<ClienteResponse>
{
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
