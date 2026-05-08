using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.DeleteItemEstoque;

public class DeleteItemEstoqueCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
