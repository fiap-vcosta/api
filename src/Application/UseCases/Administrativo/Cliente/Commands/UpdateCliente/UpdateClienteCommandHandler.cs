using Application.UseCases.Administrativo.Cliente.Responses;
using Domain.Exceptions;

using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Commands.UpdateCliente;

public class UpdateClienteCommandHandler(IClienteGateway clienteGateway)
    : IRequestHandler<UpdateClienteCommand, ClienteResponse>
{
    public async Task<ClienteResponse> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            throw new DomainNotFoundException($"Cliente com id {request.Id} não encontrado");
        }
        
        var existingCliente = await clienteGateway.GetByDocumentoAsync(request.Documento);
        if (existingCliente != null && existingCliente.Id != cliente.Id)
        {
            throw new BusinessRuleException("Já existe um cliente com este documento.");
        }

        cliente.Nome = request.Nome;
        cliente.TipoDocumento = request.TipoDocumento;
        cliente.Documento = request.Documento;

        await clienteGateway.UpdateAsync(cliente);

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
