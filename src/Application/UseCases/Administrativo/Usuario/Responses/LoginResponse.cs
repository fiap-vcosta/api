namespace Application.UseCases.Administrativo.Usuario.Responses;

public record LoginResponse
{
    public required string Token { get; init; }
}
