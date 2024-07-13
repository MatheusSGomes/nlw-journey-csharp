using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Infrastructure;

public class JourneyDbContext : DbContext
{
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Activity> Activities { get; set; } // Habilitado por conta do SqLite

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=/home/matheus/ESTUDOS/PROGRAMACAO/DOTNET/NLW/Viagens/JourneyDatabase.db");
    }

    // Não funciona com banco SqLite. Funciona com Postgres, SqlServer, MySQL.
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //
    //     modelBuilder.Entity<Activity>().ToTable("Activities");
    // }
}