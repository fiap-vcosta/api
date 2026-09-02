using Application.UseCases.Administrativo.Cliente.Responses;
using Application.Abstractions.Gateways;
using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQueryHandler(IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway)
    : IRequestHandler<GetClienteByIdQuery, ClienteResponse?>
{
    public async Task<ClienteResponse?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            return null;
        }

        var veiculos = await veiculoGateway.GetByClienteIdAsync(request.Id);

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento,
            Veiculos = veiculos.Select(v => new VeiculoResponse
            {
                Id = v.Id,
                Placa = v.Placa,
                IdCliente = v.IdCliente,
                Modelo = v.Modelo,
                Marca = v.Marca
            }).ToList()
        };
    }
}
