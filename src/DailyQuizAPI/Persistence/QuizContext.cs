using DailyQuizAPI.Features.AppSettings;
using DailyQuizAPI.Features.FriendRequests;
using DailyQuizAPI.Features.SumotHistories;
using DailyQuizAPI.Features.Sumots;
using DailyQuizAPI.Features.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DailyQuizAPI.Persistence;

public sealed class QuizContext(DbContextOptions<QuizContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Sumot> Sumots { get; set; }

    public DbSet<AppSetting> AppSettings { get; set; }

    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DbSet<SumotHistory> SumotHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Sumot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Word).IsRequired();
        });


        builder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(appS => appS.Key);
            entity.Property(appS => appS.Value).IsRequired();
        });

        builder.Entity<User>(entity =>
        {
            entity.HasMany<SumotHistory>("_sumotHistories")
                .WithOne(sh => sh.User)
                .HasForeignKey(sh => sh.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(h => h.SumotHistories);
        });

        builder.Entity<RefreshToken>(b =>
        {
            b.HasKey(rt => rt.Id);

            b.HasOne(rt => rt.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(rt => rt.UserId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SumotHistory>(entity =>
        {
            entity.HasKey(sh => sh.Id);

            entity.Ignore(sh => sh.Tries);

            entity.Property<List<string>>("_tries")
                .HasColumnName("Tries")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .HasColumnType("jsonb");
        });

        builder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(fr => fr.Id);

            entity.HasOne(fr => fr.Requester)
                .WithMany()
                .HasForeignKey(fr => fr.RequesterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(fr => new { fr.RequesterId, fr.ReceiverId }).IsUnique();
        });

    }
}