using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Enums;

namespace MyTCGBinder.Infrastructure.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserCard> UserCards => Set<UserCard>();
    public DbSet<TCGCard> TCGCards => Set<TCGCard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserCard>()
            .Property(uc => uc.Variant)
            .HasConversion<string>();

        modelBuilder.Entity<UserCard>()
            .HasIndex(uc => new { uc.UserId, uc.TcgCardId, uc.Variant })
            .IsUnique();

        modelBuilder.Entity<UserCard>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.Cards)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserCard>()
            .HasOne(uc => uc.TCGCard)
            .WithMany(c => c.UserCards)
            .HasForeignKey(uc => uc.TcgCardId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TCGCard>()
            .HasIndex(c => c.Name);

        modelBuilder.Entity<TCGCard>()
            .HasIndex(c => c.SetId);

        modelBuilder.Entity<TCGCard>()
            .HasIndex(c => new { c.SetId, c.Number });
    }
}