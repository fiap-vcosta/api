using Domain.Estoque.Entities;
using MediatR;

namespace Domain.Estoque.Events;

public record ItensNecessariosTravadosEvent(ItemEstoqueAggregateRoot ItemEstoqueAggregateRoot) : INotification;
