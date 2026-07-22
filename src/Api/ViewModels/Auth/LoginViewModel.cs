namespace Api.ViewModels.Auth;

public record LoginViewModel
{
    public required string Token { get; init; }
}
