using Application.UseCases.OrdemServico.Responses;
using MediatR;

namespace Application.UseCases.OrdemServico.Queries.GetTempoMedioAllServicos;

public class GetTempoMedioExecucaoAllServicosQuery : IRequest<List<TempoMedioExecucaoResponse>>;