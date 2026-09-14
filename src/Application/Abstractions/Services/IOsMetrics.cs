namespace Application.Abstractions.Services;

public interface IOsMetrics
{
    void IncrementCriada();

    void IncrementEvento(string evento);
}
