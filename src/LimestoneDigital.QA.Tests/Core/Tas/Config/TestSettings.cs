namespace LimestoneDigital.QA.Core.Tas.Config;

public class TestSettings
{
    public UiSettings Ui { get; set; } = new();
    public ApiSettings Api { get; set; } = new();
}

public class UiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public bool Headless { get; set; }
    public int ImplicitWaitSeconds { get; set; } = 5;
    public string StandardUser { get; set; } = string.Empty;
    public string StandardPassword { get; set; } = string.Empty;
}

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}
