namespace Api.Controllers.OrdemServico.AprovarServicosParcialmente;

public class AprovarServicosParcialmenteRequest
{
    public List<int> IdsServicosAprovados { get; init; } = [];
}