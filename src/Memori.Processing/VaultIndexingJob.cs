using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public sealed class VaultIndexingJob
{
    private readonly ILogger _logger;
    private readonly VaultIndexingJobDescription _description;
    private readonly DatabaseContext _database;

    public VaultIndexingJob(ILogger<VaultIndexingJob> logger, VaultIndexingJobDescription description, DatabaseContext database)
    {
        _logger = logger;
        _description = description;
        _database = database;
    }

    public async Task RunAsync()
    {
        using var _ = _logger.BeginScoped(("VaultId", _description.VaultId));

        _logger.LogInformation("Vault indexing job started");

        _logger.LogInformation("Finding vault");
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

        _logger.LogInformation($"Found {directories.Count()} sub directories.");

        foreach (var directory in directories)
        {
            await ProcessDirectory(vault, rootDirectory, directory);
        }

        _logger.LogInformation($"Vault indexing job complete.");
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