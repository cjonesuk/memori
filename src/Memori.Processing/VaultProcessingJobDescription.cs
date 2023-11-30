namespace Memori.Processing;

public record VaultProcessingJobDescription(string VaultId) : IProcessingJobDescription;

public record ProcessAllVaultsJobDescription() : IProcessingJobDescription;