namespace Api.Contracts.Validation;

public interface IValidator<in TRequestInstace>
{
    ValidationResult Validate(TRequestInstace instance);
}
