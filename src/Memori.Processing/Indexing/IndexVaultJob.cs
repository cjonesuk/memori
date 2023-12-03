using Memori.Data;
using Memori.Processing.Thumbnails;
using Microsoft.Extensions.Logging;

namespace Memori.Processing.Indexing;

public sealed class IndexVaultJob
{
    private readonly ILogger _logger;
    private readonly IndexVaultJobDescription _description;
    private readonly DatabaseContext _database;
    private readonly IJobManager _jobManager;

    public IndexVaultJob(
        ILogger<IndexVaultJob> logger,
        DatabaseContext database,
        IJobManager jobManager,
        IndexVaultJobDescription description)
    {
        _logger = logger;
        _description = description;
        _database = database;
        _jobManager = jobManager;
    }

    public async Task RunAsync()
    {
        using var _ = _logger.BeginScoped(("VaultId", _description.VaultId));

        _logger.LogInformation("Vault indexing job started");

        _logger.LogDebug("Finding vault");
        var vault = await _database.FindVaultByIdAsync(_description.VaultId);

        if (vault == null)
        {
            _logger.LogError($"Vault not found.");
            return;
        }

        _logger.LogDebug("Vault found.");

        var rootDirectory = new DirectoryInfo(vault.OriginalsPath);

        if (!rootDirectory.Exists)
        {
            _logger.LogError($"Vault originals path [{vault.OriginalsPath}] not found.");
            return;
        }

        await ProcessDirectory(vault, rootDirectory, rootDirectory);

        var directories = rootDirectory.EnumerateDirectories("*", SearchOption.AllDirectories);

        _logger.LogDebug($"Found {directories.Count()} sub directories.");

        foreach (var directory in directories)
        {
            await ProcessDirectory(vault, rootDirectory, directory);
        }

        RequestThumbnailGeneration(vault);

        _logger.LogInformation($"Vault indexing job complete.");
    }

    private void RequestThumbnailGeneration(Vault? vault)
    {
        _logger.LogDebug("Requesting thumbnail generation.");

        var success = _jobManager.RequestJob(new GenerateThumbnailsJobDescription(vault.Id));

        if (!success)
        {
            _logger.LogWarning("Failed to request thumbnail generation job.");
        }
    }

    private async Task ProcessDirectory(Vault? vault, DirectoryInfo rootDirectory, DirectoryInfo directory)
    {
        var assetPath = Path.GetRelativePath(rootDirectory.FullName, directory.FullName);

        _logger.LogInformation($"Indexing directory [{directory.FullName}], asset path [{assetPath}].");

        var assets = await _database.GetAssetsByPathAsync(vault.Id, assetPath);

        foreach (var file in directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
        {
            // TODO: filter out non-image files

            if (!AssetUtilities.IsImageFile(file))
            {
                _logger.LogInformation($"File [{file.Name}] is not a supported image file, ignoring.");
                continue;
            }

            var asset = assets.Find(a => a.Name == file.Name);

            if (asset == null)
            {
                _logger.LogInformation($"Asset [{file.Name}] not found. Creating new asset.");

                var metadata = await AssetUtilities.ExtractMetadata(file);
                var hash = await AssetUtilities.HashFileContents(file);


                asset = new Asset
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = metadata.Name,
                    Path = assetPath,
                    Size = metadata.Size,
                    Hash = hash.Value,
                    Created = metadata.Created, // TODO: change to use exif data
                    FileCreated = metadata.Created,
                    FileModified = metadata.Modified,
                    FileExtension = metadata.FileExtension,
                    VaultId = vault.Id,
                };

                _database.Assets.Add(asset);
            }
            else
            {
                _logger.LogInformation($"Asset [{file.Name}] found, ignoring.");

                // TODO: Detect changes to file and update asset

                //asset.Size = file.Length;
                //asset.FileCreated = file.CreationTimeUtc;
                //asset.FileModified = file.LastWriteTimeUtc;
            }

            await _database.SaveChangesAsync();
        }
    }
}