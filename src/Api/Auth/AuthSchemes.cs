namespace Api.Auth;

public static class AuthSchemes
{
    public const string Cliente = "Cliente";
}

public static class AuthPolicies
{
    public const string ClienteJwt = "ClienteJwt";
}

public static class ClientJwtClaims
{
    public const string Cpf = "cpf";
}
