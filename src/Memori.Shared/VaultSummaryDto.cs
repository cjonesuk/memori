namespace Memori.Shared;

public record VaultSummaryDto(string Id, string Name);

public record AssetSummaryDto(string Id, string Url, string ThumbnailUrl);

public record VaultDto(string Id, string Name, AssetSummaryDto[] Assets);