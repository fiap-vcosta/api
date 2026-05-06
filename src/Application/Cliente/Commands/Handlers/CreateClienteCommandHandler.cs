using Domain.Repositories;
using MediatR;

namespace Application.Cliente.Commands.Handlers;

public class CreateClienteCommandHandler(IClienteRepository clienteRepository)
    : IRequestHandler<CreateClienteCommand, ClienteResponse>
{
    public async Task<ClienteResponse> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var existingCliente = await clienteRepository.GetByDocumentoAsync(request.Documento);

        if (existingCliente != null)
        {
            throw new InvalidOperationException("Já existe um cliente com este documento.");
        }

        var cliente = new Domain.Entities.Cliente
        {
            Nome = request.Nome,
            TipoDocumento = request.TipoDocumento,
            Documento = request.Documento
        };

        await clienteRepository.CreateAsync(cliente);

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = (int)cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
