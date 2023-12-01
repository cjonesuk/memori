using Memori.Shared;

namespace Memori.Web;

public class MemoriApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync()
    {
        return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast") ?? [];
    }

    public async Task<VaultSummaryDto[]> GetVaultsAsync()
    {
        return await httpClient.GetFromJsonAsync<VaultSummaryDto[]>("/vaults") ?? [];
    }

    public async Task<VaultDto> GetVaultAsync(string vaultId)
    {
        return await httpClient.GetFromJsonAsync<VaultDto>($"/vaults/{vaultId}") ?? throw new Exception("Vault response was empty");
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


