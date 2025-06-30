using DailyQuizAPI.AppSettings;
using DailyQuizAPI.Sumots;
using DailyQuizAPI.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DailyQuizAPI.Persistence;

public sealed class QuizContext(DbContextOptions<QuizContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Sumot> Sumots { get; set; }

    public DbSet<AppSetting> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Sumot>().HasKey(s => s.Id);
        builder.Entity<Sumot>().Property(s => s.Word).IsRequired();

        builder.Entity<AppSetting>().HasKey(a => a.Key);
        builder.Entity<AppSetting>().Property(a => a.Value).IsRequired();

        builder.Entity<User>(entity =>
        {
            entity.HasMany<SumotHistory>("_sumotHistories")
                .WithOne(h => h.User)
                .HasForeignKey(h => h.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(h => h.SumotHistories);
        });

        builder.Entity<SumotHistory>(entity =>
        {
            entity.HasKey(h => h.Id);

            entity.Ignore(h => h.Tries);

            entity.Property<List<string>>("_tries")
                .HasColumnName("Tries")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .HasColumnType("jsonb");
        });
    }
}