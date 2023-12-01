using Memori.Data;
using Memori.Shared;

namespace Memori.ApiService.Vaults;

public static class VaultEndpoints
{
    public static void MapVaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/vaults", GetVaults);
        endpoints.MapGet("/vaults/{vaultId}", GetVault);
    }

    public static async Task<VaultDto> GetVault(DatabaseContext database, string vaultId)
    {
        var vault = await database.FindVaultByIdAsync(vaultId);

        if (vault == null)
        {
            throw new Exception("Vault not found");
        }

        var assets = await database.GetAssetsByVault(vaultId);

        return new VaultDto(vault.Id, vault.Name, assets.Select(asset =>
        {
            var url = $"/vaults/{vault.Id}/assets/{asset.Id}/data";
            return new AssetSummaryDto(asset.Id, url);
        }).ToArray());

    }

    public static async Task<IEnumerable<VaultSummaryDto>> GetVaults(DatabaseContext database)
    {
        var vaults = await database.FindAllVaultsAsync();

        return vaults.Select(vault => new VaultSummaryDto(vault.Id, vault.Name));
    }
}
