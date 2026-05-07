using MediatR;

namespace Application.Estoque.ItemEstoque.Commands;

public class DeleteItemEstoqueCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
