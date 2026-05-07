using Application.Administrativo.Cliente.Commands;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.Handlers;

public class GetAllClientesQueryHandler(IClienteRepository clienteRepository)
    : IRequestHandler<GetAllClientesQuery, IEnumerable<ClienteResponse>>
{
    public async Task<IEnumerable<ClienteResponse>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.GetAllAsync();
        return clientes.Select(c => new ClienteResponse
        {
            Id = c.Id,
            Nome = c.Nome,
            TipoDocumento = (int)c.TipoDocumento,
            Documento = c.Documento
        }).ToList();
    }
}
