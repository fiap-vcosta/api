using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Servico.Commands.Handlers;

public class CreateServicoCommandHandler(IServicoRepository servicoRepository)
    : IRequestHandler<CreateServicoCommand, ServicoResponse>
{
    public async Task<ServicoResponse> Handle(CreateServicoCommand request, CancellationToken cancellationToken)
    {
        var existingServico = await servicoRepository.GetByCodigoAsync(request.Codigo);
        if (existingServico != null)
        {
            throw new InvalidOperationException("Já existe um serviço com este código.");
        }

        var servico = new Domain.Administrativo.Entities.Servico
        {
            Codigo = request.Codigo,
            Nome = request.Nome,
            PrecoPadrao = request.PrecoPadrao,
            Ativo = request.Ativo
        };

        await servicoRepository.CreateAsync(servico);

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
