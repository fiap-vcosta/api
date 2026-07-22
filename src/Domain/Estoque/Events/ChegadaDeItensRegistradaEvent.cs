using Domain.Estoque.Entities;

namespace Domain.Estoque.Events;

public record ChegadaDeItensRegistradaEvent(ItemEstoqueAggregateRoot ItemEstoqueAggregateRoot);
