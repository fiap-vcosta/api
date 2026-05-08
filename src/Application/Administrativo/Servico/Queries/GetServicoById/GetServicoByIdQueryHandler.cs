using Application.Administrativo.Servico.Commands.CreateServico;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Servico.Queries.GetServicoById;

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
