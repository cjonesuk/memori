namespace Memori.Data;

public static class AssetExtensions
{
    public static async Task<List<Asset>> GetAssetsByPathAsync(this DatabaseContext database, string vaultId, string path)
    {
        var assets = await database.Assets
            .Include(a => a.Vault)
            .Where(a => a.Vault.Id == vaultId && a.Path == path)
            .ToListAsync();

        return assets;

    }

    public static async Task<Asset?> FindAssetByPathAsync(this DatabaseContext database, string path)
    {
        var vault = await database.Assets
            .Include(a => a.Vault)
            .FirstOrDefaultAsync(a => a.Path == path);

        return vault;
    }
}
