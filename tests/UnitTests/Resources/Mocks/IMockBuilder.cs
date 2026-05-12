namespace UnitTests.Resources.Mocks;

public interface IMockBuilder<out TMock>
{
    public TMock Build();
}