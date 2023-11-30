namespace Memori.Processing;

public record JobRequest(Guid JobRequestId, IProcessingJobDescription Description);
