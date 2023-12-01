using System.Net.Http.Json;

namespace Memori.Shared;

public class MemoriApiClient(HttpClient httpClient)
{
    public async Task<VaultSummaryDto[]> GetVaultsAsync()
    {
        return await httpClient.GetFromJsonAsync<VaultSummaryDto[]>("/vaults") ?? [];
    }

    public async Task<VaultDto> GetVaultAsync(string vaultId)
    {
        return await httpClient.GetFromJsonAsync<VaultDto>($"/vaults/{vaultId}") ?? throw new Exception("Vault response was empty");
    }
}