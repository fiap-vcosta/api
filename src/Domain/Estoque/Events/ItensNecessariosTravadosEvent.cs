using Domain.Estoque.Entities;
using MediatR;

namespace Domain.Estoque.Events;

public record ItensNecessariosTravadosEvent(ItemEstoque ItemEstoque) : INotification;
