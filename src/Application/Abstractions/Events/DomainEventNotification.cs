using MediatR;

namespace Application.Abstractions.Events;

public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : class;
