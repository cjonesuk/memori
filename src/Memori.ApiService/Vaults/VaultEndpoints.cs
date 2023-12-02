using Memori.Data;
using Memori.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Memori.ApiService.Vaults;

public static class VaultEndpoints
{
    public static void MapVaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/vaults", GetVaults);
        endpoints.MapGet("/vaults/{vaultId}", GetVault);
        endpoints.MapGet("/vaults/{vaultId}/assets/{assetId}/data", GetAssetFileContentAsync);
    }

    public static async Task<VaultDto> GetVault([FromServices] DatabaseContext database, HttpRequest request, [FromRoute] string vaultId)
    {
        var vault = await database.FindVaultByIdAsync(vaultId);

        if (vault == null)
        {
            throw new Exception("Vault not found");
        }

        var assets = await database.GetAssetsByVault(vaultId);



        return new VaultDto(vault.Id, vault.Name, assets.Select(asset =>
        {
            var path = $"/vaults/{vault.Id}/assets/{asset.Id}/data";
            var url = request.GenerateUrlFromBase(path);
            return new AssetSummaryDto(asset.Id, url);
        }).ToArray());

    }

    public static async Task<IEnumerable<VaultSummaryDto>> GetVaults(DatabaseContext database)
    {
        var vaults = await database.FindAllVaultsAsync();

        return vaults.Select(vault => new VaultSummaryDto(vault.Id, vault.Name));
    }

    public static async Task<IResult> GetAssetFileContentAsync(
        [FromServices] DatabaseContext database,
        [FromServices] ILogger logger,
        [FromRoute] string vaultId,
        [FromRoute] string assetId)
    {
        var asset = await database.FindAssetByIdAsync(vaultId, assetId);

        if (asset == null)
        {
            return Results.NotFound();
        }

        logger.LogInformation($"Vault originals path {asset.Vault.OriginalsPath}");
        logger.LogInformation($"Asset file path {asset.Path}");

        var path = Path.Combine(asset.Vault.OriginalsPath, asset.Path, asset.Name);

        logger.LogInformation($"Serving asset file {path}");

        var stream = File.OpenRead(path);

        return Results.File(stream, contentType: "image/jpeg");
    }
}

public static class HttpRequestExtensions
{
    public static string GenerateUrlFromBase(this HttpRequest req, string path)
    {
        var baseUrl = req.BaseUrl();

        baseUrl.Path = path;

        return baseUrl.Uri.AbsoluteUri;

    }

    public static UriBuilder BaseUrl(this HttpRequest req)
    {
        var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
        if (uriBuilder.Uri.IsDefaultPort)
        {
            uriBuilder.Port = -1;
        }

        return uriBuilder;
    }
}