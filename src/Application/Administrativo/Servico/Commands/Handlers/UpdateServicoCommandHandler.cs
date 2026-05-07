using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Servico.Commands.Handlers;

public class UpdateServicoCommandHandler(IServicoRepository servicoRepository)
    : IRequestHandler<UpdateServicoCommand, ServicoResponse>
{
    public async Task<ServicoResponse> Handle(UpdateServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await servicoRepository.GetByIdAsync(request.Id);
        if (servico == null)
        {
            throw new KeyNotFoundException($"Serviço com id {request.Id} não encontrado");
        }

        var existingServico = await servicoRepository.GetByCodigoAsync(request.Codigo);
        if (existingServico != null && existingServico.Id != servico.Id)
        {
            throw new InvalidOperationException("Já existe um serviço com este código.");
        }

        servico.Codigo = request.Codigo;
        servico.Nome = request.Nome;
        servico.PrecoPadrao = request.PrecoPadrao;
        servico.Ativo = request.Ativo;

        await servicoRepository.UpdateAsync(servico);

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
