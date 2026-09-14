using Api.Logging;

namespace UnitTests.Api.Logging;

public class DatadogLoggingTests
{
    [Theory]
    [InlineData("datadoghq.com", "https://http-intake.logs.datadoghq.com")]
    [InlineData("us5.datadoghq.com", "https://http-intake.logs.us5.datadoghq.com")]
    [InlineData("datadoghq.eu", "https://http-intake.logs.datadoghq.eu")]
    [InlineData("  us5.datadoghq.com  ", "https://http-intake.logs.us5.datadoghq.com")]
    public void ResolveLogsIntakeUrl_DerivesIntakeFromDdSite(string ddSite, string expected)
    {
        // Arrange / Act
        var intakeUrl = DatadogLogging.ResolveLogsIntakeUrl(ddSite);

        // Assert
        Assert.Equal(expected, intakeUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ResolveLogsIntakeUrl_ReturnsNull_WhenDdSiteMissing(string? ddSite)
    {
        // Arrange / Act
        var intakeUrl = DatadogLogging.ResolveLogsIntakeUrl(ddSite);

        // Assert
        Assert.Null(intakeUrl);
    }
}
