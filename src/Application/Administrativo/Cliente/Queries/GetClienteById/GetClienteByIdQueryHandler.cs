using Application.Administrativo.Cliente.Commands.CreateCliente;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQueryHandler(IClienteRepository clienteRepository)
    : IRequestHandler<GetClienteByIdQuery, ClienteResponse?>
{
    public async Task<ClienteResponse?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            return null;
        }

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
