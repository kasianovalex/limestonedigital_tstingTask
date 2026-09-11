using Microsoft.Extensions.Configuration;

namespace LimestoneDigital.QA.Core.Tas.Config;

/// <summary>
/// Layers appsettings.json with an optional appsettings.{TEST_ENV}.json overlay, then
/// environment variables (prefix QA_, "__" as section separator) so CI/secret stores can
/// override any value without touching the committed files.
/// </summary>
public static class ConfigurationProvider
{
    private static readonly Lazy<TestSettings> LazySettings = new(Load);

    public static TestSettings Settings => LazySettings.Value;

    private static TestSettings Load()
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENV") ?? "dev";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables(prefix: "QA_")
            .Build();

        var settings = new TestSettings();
        configuration.Bind(settings);
        return settings;
    }
}
