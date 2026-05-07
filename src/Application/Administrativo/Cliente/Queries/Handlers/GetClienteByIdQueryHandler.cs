using Application.Administrativo.Cliente.Commands;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.Handlers;

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
            TipoDocumento = (int)cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
