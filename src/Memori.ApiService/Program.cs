using Memori.ApiService.Jobs;
using Memori.ApiService.Vaults;
using Memori.Data;
using Memori.Processing;
using Memori.Processing.Indexing;
using Memori.ServiceDefaults;

Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddSqlServerDbContext<DatabaseContext>(Constants.SqlServerDatabase);

builder.Services.AddTransient<DataMigrationJob>();

builder.Services.AddJobManager();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();  //set the allowed origin  
        });
});

builder.Services.AddTransient<ILogger>(p =>
{
    var loggerFactory = p.GetRequiredService<ILoggerFactory>();
    // You could also use the HttpContext to make the name dynamic for example
    return loggerFactory.CreateLogger("api-logger");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var job = scope.ServiceProvider.GetRequiredService<DataMigrationJob>();
    await job.RunAsync();
}



// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseCors();

app.MapVaultEndpoints();

app.MapPost("/import", (IJobManager processingManager) =>
{
    var success = processingManager.RequestJob(new IndexAllVaultsJobDescription());

    if (!success)
    {
        return Results.Problem("The request was not accepted.");
    }

    return Results.Accepted();
});//.Produces<ProblemDetails>(StatusCodes.Status202Accepted, "The request has been accepted for processing but the processing has not been completed.");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.Run();
