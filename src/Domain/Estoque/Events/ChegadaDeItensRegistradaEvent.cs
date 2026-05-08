using Domain.Estoque.Entities;
using MediatR;

namespace Domain.Estoque.Events;

public record ChegadaDeItensRegistradaEvent(ItemEstoqueAggregateRoot ItemEstoqueAggregateRoot): INotification;
