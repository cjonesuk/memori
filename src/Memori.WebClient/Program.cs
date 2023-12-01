using Memori.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient<MemoriApiClient>(client =>
{
    client.BaseAddress = new("http://localhost:5388");
});


await builder.Build().RunAsync();
