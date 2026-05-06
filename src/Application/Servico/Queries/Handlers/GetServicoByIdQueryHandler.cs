using Application.Servico.Commands;
using Domain.Repositories;
using MediatR;

namespace Application.Servico.Queries.Handlers;

public class GetServicoByIdQueryHandler(IServicoRepository servicoRepository)
    : IRequestHandler<GetServicoByIdQuery, ServicoResponse?>
{
    public async Task<ServicoResponse?> Handle(GetServicoByIdQuery request, CancellationToken cancellationToken)
    {
        var servico = await servicoRepository.GetByIdAsync(request.Id);
        if (servico == null)
        {
            return null;
        }

        return new ServicoResponse
        {
            Id = servico.Id,
            Codigo = servico.Codigo,
            Nome = servico.Nome,
            PrecoPadrao = servico.PrecoPadrao,
            Ativo = servico.Ativo
        };
    }
}
