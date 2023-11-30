namespace Memori.Data;

public static class VaultExtensions
{
    public static async Task<Vault?> FindVaultByIdAsync(this DatabaseContext database, string id)
    {
        var vault = await database.Vaults.FirstOrDefaultAsync(v => v.Id == id);

        return vault;
    }
}