using DailyQuizAPI.Sumots;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Persistence;

public class QuizContext(DbContextOptions<QuizContext> options) : DbContext(options)
{
    public DbSet<Sumot> Sumots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sumot>().HasKey(s => s.Id);
        modelBuilder.Entity<Sumot>().Property(s => s.Word).IsRequired();
        modelBuilder.Entity<Sumot>().Property(s => s.Day);
    }
}