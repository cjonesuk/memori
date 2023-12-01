using Memori.ApiService.Jobs;
using Memori.ApiService.Vaults;
using Memori.Data;
using Memori.Processing;
using Memori.ServiceDefaults;

Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddSqlServerDbContext<DatabaseContext>(Constants.SqlServerDatabase);

builder.Services.AddTransient<DataMigrationJob>();

builder.Services.AddProcessingManager();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var job = scope.ServiceProvider.GetRequiredService<DataMigrationJob>();
    await job.RunAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
};

app.MapGet(
    "/weatherforecast",
    () =>
    {
        var forecast = Enumerable
            .Range(1, 5)
            .Select(
                index =>
                    new WeatherForecast(
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    )
            )
            .ToArray();
        return forecast;
    }
);

app.MapVaultEndpoints();


app.MapPost("/import", (IProcessingManagerBackgroundService processingManager) =>
{
    var success = processingManager.RequestJob(new ProcessAllVaultsJobDescription());

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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
