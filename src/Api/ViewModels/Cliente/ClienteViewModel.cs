using Domain.Administrativo.Entities;

namespace Api.ViewModels.Cliente;

public record ClienteViewModel
{
    public required int Id { get; init; }
    public required string Nome { get; init; }
    public required TipoDocumento TipoDocumento { get; init; }
    public required string Documento { get; init; }
}
