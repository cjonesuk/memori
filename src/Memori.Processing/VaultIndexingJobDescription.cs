namespace Memori.Processing;

public record VaultIndexingJobDescription(string VaultId) : IProcessingJobDescription;

public record ProcessAllVaultsJobDescription() : IProcessingJobDescription;