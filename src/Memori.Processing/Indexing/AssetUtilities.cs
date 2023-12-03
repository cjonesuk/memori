using System.Security.Cryptography;

namespace Memori.Processing.Indexing;

public static class AssetUtilities
{
    public static async Task<FileHash> HashFileContents(FileInfo file)
    {
        using var sha256 = SHA256.Create();
        await using var stream = file.OpenRead();
        var hash = await sha256.ComputeHashAsync(stream);
        var value = Convert.ToBase64String(hash);

        return new FileHash { Value = value };
    }

    public static async Task<FileMetadata> ExtractMetadata(FileInfo file)
    {
        return new FileMetadata
        {
            Name = file.Name,
            FileExtension = file.Extension,
            Size = file.Length,
            Created = file.CreationTimeUtc,
            Modified = file.LastWriteTimeUtc
        };
    }

    private static readonly string[] ValidExtensions = [".jpg", ".jpeg", ".png"];

    public static bool IsImageFile(FileInfo file) =>
        ValidExtensions.Contains(file.Extension.ToLowerInvariant());
}
