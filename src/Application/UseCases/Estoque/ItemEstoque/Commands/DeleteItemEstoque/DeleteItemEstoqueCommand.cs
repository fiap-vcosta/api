using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.DeleteItemEstoque;

public class DeleteItemEstoqueCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
