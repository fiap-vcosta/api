using Domain.OrdemServico.ValueObjects;

namespace Application.UseCases.OrdemServico.Responses;

public class ClienteOrdemServicoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public static ClienteOrdemServicoResponse From(ClienteOrdemServico cliente)
    {
        return new ClienteOrdemServicoResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email
        };
    }
}
