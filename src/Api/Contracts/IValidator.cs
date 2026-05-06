namespace Api.Contracts;

public interface IValidator<in TRequestInstace>
{
    ValidationResult Validate(TRequestInstace instance);
}
