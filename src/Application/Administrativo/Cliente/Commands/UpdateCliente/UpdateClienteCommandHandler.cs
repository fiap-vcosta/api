using Application.Administrativo.Cliente.Commands.CreateCliente;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Commands.UpdateCliente;

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
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
