namespace Conductor.Application.Configuration;

public class DatabaseConfig
{
    public DatabaseType Type { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
