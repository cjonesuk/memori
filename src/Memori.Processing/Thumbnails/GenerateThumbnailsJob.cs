using ImageMagick;
using Memori.Data;
using Memori.Processing.Thumbnails;
using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public sealed class GenerateThumbnailsJob
{
    private readonly ILogger _logger;
    private readonly GenerateThumbnailsJobDescription _description;
    private readonly DatabaseContext _database;

    public GenerateThumbnailsJob(ILogger<GenerateThumbnailsJob> logger, GenerateThumbnailsJobDescription description, DatabaseContext database)
    {
        _logger = logger;
        _description = description;
        _database = database;
    }

    public async Task RunAsync()
    {
        using var _ = _logger.BeginScoped(("VaultId", _description.VaultId));

        _logger.LogInformation("Generate thumbnails job started");

        _logger.LogDebug("Finding vault");
        var vault = await _database.FindVaultByIdAsync(_description.VaultId);

        if (vault == null)
        {
            _logger.LogError($"Vault not found.");
            return;
        }

        _logger.LogDebug("Vault found.");


        var assets = await _database.FindAssetsWithoutThumbnails(_description.VaultId);

        _logger.LogDebug($"Found {assets.Count()} assets without thumbnails.");

        foreach (var asset in assets)
        {
            await ProcessAsset(vault, asset);
        }

        _logger.LogInformation("Generate thumbnails job complete.");
    }

    private async Task ProcessAsset(Vault vault, Asset asset)
    {
        using var _ = _logger.BeginScoped(("AssetId", asset.Id));

        _logger.LogInformation($"Generating thumbnails for asset {asset.Id}.");

        var thumbnailName = $"{asset.Id}.thumb.jpg";

        var filePath = Path.Combine(vault.OriginalsPath, asset.Path, asset.Name);
        var thumbnailFilePath = Path.Combine("../../data/thumbnails", thumbnailName);

        var file = new FileInfo(filePath);

        if (!file.Exists)
        {
            _logger.LogError($"File {filePath} does not exist.");
            // TODO: how to handle unhappy path?
            return;
        }

        var thumbnailFile = new FileInfo(thumbnailFilePath);
        if (thumbnailFile.Exists)
        {
            _logger.LogWarning($"Thumbnail file {thumbnailFilePath} already exists, deleting...");
            thumbnailFile.Delete();

            // TODO: unhappy path
        }

        if (thumbnailFile.Directory is not null)
        {
            _logger.LogInformation($"Creating directory {thumbnailFile.Directory.FullName}.");
            thumbnailFile.Directory.Create();
        }

        // use image magic to generate thumbnails
        var result = await GenerateThumbnail(file, thumbnailFile);

        if (result == ThumbnailResult.Failed)
        {
            _logger.LogError($"Failed to generate thumbnail for file {file.FullName}.");
            return;
        }

        _logger.LogInformation($"Generated thumbnail for asset {asset.Id}.");

        asset.ThumbnailPath = thumbnailName;

        _logger.LogDebug("Saving thumbnail path to database.");
        await _database.SaveChangesAsync();
    }

    protected enum ThumbnailResult
    {
        Failed,
        Success,
    }

    private async Task<ThumbnailResult> GenerateThumbnail(FileInfo file, FileInfo thumbnailFile)
    {
        // TODO: Security policy for ImageMagick



        using var image = new MagickImage(file);

        var size = new MagickGeometry(200, 200);
        size.IgnoreAspectRatio = false;

        image.Resize(size);

        await image.WriteAsync(thumbnailFile);

        //var profile = image.GetExifProfile();

        //if (profile is null)
        //{
        //    _logger.LogDebug($"File {file.FullName} does not contain an exif profile, unable to generate thumbnail.");
        //     }

        //using var thumbnail = profile.CreateThumbnail();

        //// Check if exif profile contains thumbnail and save it
        //if (thumbnail is not null)
        //{
        //    await thumbnail.WriteAsync(thumbnailFile);
        //}

        _logger.LogInformation($"Generated thumbnail for file {file.FullName}.");

        return ThumbnailResult.Success;
    }


}
