using Memori.Shared;
using Memori.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<MemoriApiClient>(client => client.BaseAddress = new("http://apiservice"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Memori.WebClient._Imports).Assembly);

app.MapDefaultEndpoints();

app.Run();
