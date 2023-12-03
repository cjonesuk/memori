namespace Memori.Processing.Indexing;

public class FileMetadata
{
    public required string Name { get; init; }
    public required string FileExtension { get; init; }
    public required long Size { get; init; }
    public required DateTime Created { get; init; }
    public required DateTime Modified { get; init; }
}
