using Memori.ServiceDefaults;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

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


