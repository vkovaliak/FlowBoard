using DbUp;
using FlowBoard.Persistence.Configurations;
using Microsoft.Extensions.Options;

namespace FlowBoard.Database;

public class DatabaseInitializer
{
    private readonly string _connectionString;
    public DatabaseInitializer(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task InitializeAsync()
    {
        EnsureDatabase.For.SqlDatabase(_connectionString);

        var upgrader = DeployChanges.To.SqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .Build();
        
        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();
        }
    }

}
