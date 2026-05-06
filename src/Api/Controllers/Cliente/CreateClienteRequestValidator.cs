using Api.Contracts;
using CpfCnpjLibrary;

namespace Api.Controllers.Cliente;

public class CreateClienteRequestValidator : IValidator<CreateClienteRequest>
{
    public ValidationResult Validate(CreateClienteRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            result.Errors.Add("Nome não pode estar vazio.");
        }

        if (!Enum.IsDefined(typeof(Domain.Entities.TipoDocumento), request.TipoDocumento))
        {
            result.Errors.Add("TipoDocumento é inválido.");
        }

        if (string.IsNullOrWhiteSpace(request.Documento))
        {
            result.Errors.Add("Documento não pode estar vazio.");
        }
        else
        {
            var tipoDocumento = (Domain.Entities.TipoDocumento)request.TipoDocumento;
            var documentoClean = request.Documento.Replace(".", "").Replace("-", "").Replace("/", "");

            switch (tipoDocumento)
            {
                case Domain.Entities.TipoDocumento.Cpf:
                {
                    if (!Cpf.Validar(documentoClean))
                    {
                        result.Errors.Add("CPF inválido.");
                    }

                    break;
                }
                case Domain.Entities.TipoDocumento.Cnpj:
                {
                    if (!Cnpj.Validar(documentoClean))
                    {
                        result.Errors.Add("CNPJ inválido.");
                    }

                    break;
                }
            }
        }

        return result;
    }
}
