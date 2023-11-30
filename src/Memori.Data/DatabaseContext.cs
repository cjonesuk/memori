namespace Memori.Data;

using Microsoft.EntityFrameworkCore;

public class User
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Vault> Vaults { get; } = new List<Vault>();
}

public class Vault
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string OriginalsPath { get; set; }
    public required string? ImportPath { get; init; }

    public ICollection<User> Users { get; } = new List<User>();
}

public class Asset
{
    public required string Id { get; set; }
    public required string Path { get; set; }
    public required long Size { get; set; }
    public required string Hash { get; set; }
    public required DateTimeOffset FileCreated { get; set; }
    public required DateTimeOffset FileModified { get; set; }
    public required string FileExtension { get; set; }

    public required string VaultId { get; set; }
    public Vault? Vault { get; set; }
}

public class DatabaseContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Vault> Vaults => Set<Vault>();
    public DbSet<User> Users => Set<User>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasMany(e => e.Vaults).WithMany(e => e.Users);
        });

        modelBuilder.Entity<Vault>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.OriginalsPath).IsRequired();

            entity.HasMany(e => e.Users).WithMany(e => e.Vaults);
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Path).IsRequired();
            entity.Property(e => e.Size).IsRequired();
            entity.Property(e => e.Hash).IsRequired();
            entity.Property(e => e.FileCreated).IsRequired();
            entity.Property(e => e.FileModified).IsRequired();
            entity.Property(e => e.FileExtension).IsRequired();

            entity.HasOne(e => e.Vault).WithMany();
        });
    }
}
