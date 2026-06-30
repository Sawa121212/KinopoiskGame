using System.Data.Entity;
using System.Diagnostics;
using FilmsDB.Domain.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
namespace FilmsDB.Domain;

[DbConfigurationType(typeof(KinopoiskDbConfiguration))]
public sealed class KinopoiskDbContext : DbContext
{
    public Microsoft.EntityFrameworkCore.DbSet<Film?> Films { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Category> Categories { get; set; }

    public KinopoiskDbContext()
    {
        // при создании контекста автоматически проверит наличие базы данных и, если она отсутствует, создаст ее.
        bool isCreated = Database.EnsureCreated();
        Debug.WriteLine(isCreated ? "База данных была создана" : "База данных уже существует");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=films.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}