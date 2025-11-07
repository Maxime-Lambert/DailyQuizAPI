using DailyQuizAPI.Features.Crosscutting.AppSettings;
using DailyQuizAPI.Features.Crosscutting.FriendRequests;
using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Features.SumotApp.SumotHistories;
using DailyQuizAPI.Features.SumotApp.Sumots;
using DailyQuizAPI.Features.SumotApp.SumotStats;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Persistence;

public sealed class QuizContext(DbContextOptions<QuizContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Sumot> Sumots { get; set; }

    public DbSet<AppSetting> AppSettings { get; set; }

    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DbSet<SumotHistory> SumotHistories { get; set; }
    public DbSet<SumotStat> SumotStats { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Sumot>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Word).IsRequired();
            entity.HasIndex(s => s.Word);
            entity.HasIndex(s => s.Day);
        });

        builder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(appS => appS.Key);
            entity.Property(appS => appS.Value).IsRequired();
        });

        builder.Entity<User>(entity =>
        {
            entity.HasMany(h => h.SumotHistories)
                .WithOne(h => h.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(u => u.UserName)
                .IsUnique();
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

            entity.HasMany(h => h.Tries)
                .WithOne()
                .HasForeignKey(t => t.SumotHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(h => h.Word);
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
            entity.HasIndex(fr => fr.RequesterId);
            entity.HasIndex(fr => fr.ReceiverId);
        });

    }
}