using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Queries.GetTempoMedioAllServicos;

public class GetTempoMedioExecucaoAllServicosQuery : IRequest<List<IItemServicoRepository.TempoMedioExecucaoServico>>;