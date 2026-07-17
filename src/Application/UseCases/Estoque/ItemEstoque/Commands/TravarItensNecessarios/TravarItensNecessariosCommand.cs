using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.TravarItensNecessarios;

public class TravarItensNecessariosCommand : IRequest<Unit>
{
    public int IdItemEstoque { get; init; }
    public decimal QuantidadeNecessaria { get; init; }
}