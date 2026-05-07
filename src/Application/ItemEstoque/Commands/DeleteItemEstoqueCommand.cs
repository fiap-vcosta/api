using MediatR;

namespace Application.ItemEstoque.Commands;

public class DeleteItemEstoqueCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
