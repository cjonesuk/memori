namespace Memori.Data;

public static class AssetExtensions
{
    public static async Task<Asset?> FindAssetByPathAsync(this DatabaseContext database, string path)
    {
        var vault = await database.Assets
            .Include(a => a.Vault)
            .FirstOrDefaultAsync(a => a.Path == path);

        return vault;
    }
}
