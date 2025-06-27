using DailyQuizAPI.Sumots;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Persistence;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by dependency injection")]
internal sealed class QuizContext(DbContextOptions<QuizContext> options) : DbContext(options)
{
    public DbSet<Sumot> Sumots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sumot>().HasKey(s => s.Id);
        modelBuilder.Entity<Sumot>().Property(s => s.Word).IsRequired();
        modelBuilder.Entity<Sumot>().Property(s => s.Day);
    }
}