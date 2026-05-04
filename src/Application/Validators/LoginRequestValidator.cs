namespace Application.Validators;

public class LoginRequestValidator : IValidator<LoginRequest>
{
    public ValidationResult Validate(LoginRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Login))
        {
            result.Errors.Add("Login must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            result.Errors.Add("Password must not be empty.");
        }

        return result;
    }
}
