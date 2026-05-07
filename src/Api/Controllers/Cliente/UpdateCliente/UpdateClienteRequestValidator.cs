using Api.Contracts.Validation;
using CpfCnpjLibrary;
using Domain.Administrativo.Entities;

namespace Api.Controllers.Cliente.UpdateCliente;

public class UpdateClienteRequestValidator : IValidator<UpdateClienteRequest>
{
    public ValidationResult Validate(UpdateClienteRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            result.Errors.Add("Nome não pode estar vazio.");
        }

        if (!Enum.IsDefined(typeof(TipoDocumento), request.TipoDocumento))
        {
            result.Errors.Add("TipoDocumento é inválido.");
        }


        if (string.IsNullOrWhiteSpace(request.Documento))
        {
            result.Errors.Add("Documento não pode estar vazio.");
        }
        else
        {
            var tipoDocumento = (TipoDocumento)request.TipoDocumento;
            var documentoClean = request.Documento.Replace(".", "").Replace("-", "").Replace("/", "");

            switch (tipoDocumento)
            {
                case TipoDocumento.Cpf:
                {
                    if (!Cpf.Validar(documentoClean))
                    {
                        result.Errors.Add("CPF inválido.");
                    }

                    break;
                }
                case TipoDocumento.Cnpj:
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
