using Memori.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServerContainer(Constants.SqlServer, password: Constants.SqlPassword)
    .WithVolumeMount("temp/sql-data", "/var/opt/mssql/data")
    .AddDatabase(Constants.SqlServerDatabase);

var apiService = builder
    .AddProject<Projects.Memori_ApiService>(Constants.ApiService)
    .WithReference(sql);

builder
    .AddProject<Projects.Memori_Web>(Constants.WebFrontend)
    .WithReference(apiService);

builder.Build().Run();


