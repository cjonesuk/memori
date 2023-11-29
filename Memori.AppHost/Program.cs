var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.Memori_ApiService>("apiservice");

builder.AddProject<Projects.Memori_Web>("webfrontend")
    .WithReference(apiservice);

builder.Build().Run();
