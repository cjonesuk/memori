﻿using Memori.Data;
using Memori.Processing;
using Microsoft.EntityFrameworkCore;

namespace Memori.ApiService.Jobs;

public sealed class DataMigrationJob
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _database;
    private readonly IProcessingManagerBackgroundService _processingManager;

    public DataMigrationJob(ILogger<DataMigrationJob> logger, DatabaseContext database, IProcessingManagerBackgroundService processingManager)
    {
        _logger = logger;
        _database = database;
        _processingManager = processingManager;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Migrating the database...");

        await _database.Database.MigrateAsync();

        _logger.LogInformation("Database migrated.");


        _logger.LogInformation("Checking if the database needs to be seeded...");
        if (!await _database.Vaults.AnyAsync())
        {
            _logger.LogInformation("Seeding the database...");

            var vault01Id = "VAULT01".ToString();
            var userId = "USER01".ToString();

            var vault01 = new Vault
            {
                Id = vault01Id,
                Name = "My Vault 01",
                OriginalsPath = "../../data/animals",
                ImportPath = null
            };


            _database.Add(
                new User
                {
                    Id = userId,
                    Name = "John Doe",
                    Vaults = { vault01 }
                }
            );

            await _database.SaveChangesAsync();

            _logger.LogInformation("Database seeded.");

            _logger.LogInformation("Requesting processing job for all vaults...");
            _processingManager.RequestIndexAllVaults();
        }
    }
}
