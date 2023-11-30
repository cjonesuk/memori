﻿using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Memori.ApiService;

namespace Memori.AppHost;


public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        Console.WriteLine("Creating the database context...");
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=MemoriData-Migrations;Integrated Security=SSPI");

        return new DatabaseContext(optionsBuilder.Options);
    }
}
