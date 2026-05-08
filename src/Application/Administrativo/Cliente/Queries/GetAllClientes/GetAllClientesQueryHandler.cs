using Application.Administrativo.Cliente.Commands.CreateCliente;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.GetAllClientes;

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
            TipoDocumento = c.TipoDocumento,
            Documento = c.Documento
        }).ToList();
    }
}
