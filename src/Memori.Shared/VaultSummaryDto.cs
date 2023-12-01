namespace Memori.Shared;

public record VaultSummaryDto(string Id, string Name);

public record AssetSummaryDto(string Id, string Url);

public record VaultDto(string Id, string Name, AssetSummaryDto[] Assets);