namespace Memori.Data;

public static class VaultExtensions
{
    public static async Task<Vault?> FindVaultByIdAsync(this DatabaseContext database, string id)
    {
        var vault = await database.Vaults.FirstOrDefaultAsync(v => v.Id == id);

        return vault;
    }

    public static async Task<List<Vault>> FindAllVaultsAsync(this DatabaseContext database)
    {
        var vaults = await database.Vaults.ToListAsync();

        return vaults;
    }

    public static async Task<List<Asset>> GetAssetsByVault(this DatabaseContext database, string vaultId)
    {
        var assets = await database.Assets.Where(asset => asset.VaultId == vaultId).ToListAsync();

        return assets;
    }

    public static async Task<Asset?> FindAssetByIdAsync(this DatabaseContext database, string vaultId, string assetId)
    {
        var asset = await database.Assets.Include(x => x.Vault).FirstOrDefaultAsync(a => a.VaultId == vaultId && a.Id == assetId);

        return asset;
    }

    public static async Task<List<Asset>> FindAssetsWithoutThumbnails(this DatabaseContext database, string vaultId)
    {
        var assets = await database.Assets.Where(asset => asset.VaultId == vaultId && asset.ThumbnailPath == null).ToListAsync();

        return assets;
    }
}