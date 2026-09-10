using Api.ViewModels.Cliente;
using Application.UseCases.Administrativo.Cliente.Responses;

namespace Api.Presenters.Cliente;

public class ConsultarClientePorDocumentoPresenter
{
    public ConsultarClientePorDocumentoViewModel Present(ConsultarClientePorDocumentoResponse response)
    {
        return new ConsultarClientePorDocumentoViewModel
        {
            Existe = response.Existe,
            Documento = response.Documento,
            TipoDocumento = response.TipoDocumento
        };
    }
}
