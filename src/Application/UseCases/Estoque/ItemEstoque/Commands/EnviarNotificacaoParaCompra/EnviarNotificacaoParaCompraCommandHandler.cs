using Application.Abstractions.Services;
using Domain.Administrativo.Entities;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.EnviarNotificacaoParaCompra;

public class EnviarNotificacaoParaCompraCommandHandle(INotificacaoService notificacaoService)
    : IRequestHandler<EnviarNotificacaoParaCompraCommand, Unit>
{
    public async Task<Unit> Handle(EnviarNotificacaoParaCompraCommand request, CancellationToken cancellationToken)
    {
        var notificacao = $"Comprar {request.QuantidadeFaltando} de {request.NomeItem} para OS {request.IdOrdemServico}";

        await notificacaoService.NotificarUsuariosPorTipo(TipoUsuario.Atendente, notificacao);

        return Unit.Value;
    }
}
