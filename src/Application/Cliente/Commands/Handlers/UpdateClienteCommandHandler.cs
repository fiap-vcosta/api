using Domain.Repositories;
using MediatR;

namespace Application.Cliente.Commands.Handlers;

public class UpdateClienteCommandHandler(IClienteRepository clienteRepository)
    : IRequestHandler<UpdateClienteCommand, ClienteResponse>
{
    public async Task<ClienteResponse> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente com id {request.Id} não encontrado");
        }
        
        var existingCliente = await clienteRepository.GetByDocumentoAsync(request.Documento);
        if (existingCliente != null && existingCliente.Id != cliente.Id)
        {
            throw new InvalidOperationException("Já existe um cliente com este documento.");
        }

        cliente.Nome = request.Nome;
        cliente.TipoDocumento = request.TipoDocumento;
        cliente.Documento = request.Documento;

        await clienteRepository.UpdateAsync(cliente);

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = (int)cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
