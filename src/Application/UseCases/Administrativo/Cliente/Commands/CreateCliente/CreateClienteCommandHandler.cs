using Application.Abstractions.Gateways;
using Domain.Exceptions;
using Application.UseCases.Administrativo.Cliente.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Commands.CreateCliente;

public class CreateClienteCommandHandler(IClienteGateway clienteGateway)
    : IRequestHandler<CreateClienteCommand, ClienteResponse>
{
    public async Task<ClienteResponse> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var existingCliente = await clienteGateway.GetByDocumentoAsync(request.Documento);

        if (existingCliente != null)
        {
            throw new BusinessRuleException("Já existe um cliente com este documento.");
        }

        var cliente = new Domain.Administrativo.Entities.ClienteAggregateRoot
        {
            Nome = request.Nome,
            TipoDocumento = request.TipoDocumento,
            Documento = request.Documento
        };

        await clienteGateway.CreateAsync(cliente);

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
