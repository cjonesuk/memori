namespace Memori.Processing.Thumbnails;

public record GenerateThumbnailsJobDescription(string VaultId) : IJobDescription;
