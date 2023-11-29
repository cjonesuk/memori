var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServerContainer(Constants.SqlServer, password: Constants.SqlPassword)
    .AddDatabase(Constants.SqlServerDatabase);

var apiService = builder
    .AddProject<Projects.Memori_ApiService>(Constants.ApiService)
    .WithReference(sql);

builder
    .AddProject<Projects.Memori_Web>(Constants.WebFrontend)
    .WithReference(apiService);

builder.Build().Run();


static class Constants
{
    public const string SqlServer = "sql";
    public const string SqlServerDatabase = "MemoriData";
    public const string SqlPassword = "SecurePass123*";

    public const string ApiService = "apiservice";
    public const string WebFrontend = "webfrontend";
}
