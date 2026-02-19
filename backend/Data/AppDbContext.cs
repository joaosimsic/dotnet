using Microsoft.EntityFrameworkCore;
using PhoneBook.Api.Models;

namespace PhoneBook.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Contact> Contacts => Set<Contact>();
  public DbSet<Phone> Phones => Set<Phone>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Contact>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).UseIdentityColumn();
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    });

    modelBuilder.Entity<Phone>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).UseIdentityColumn();
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
      entity.HasOne(e => e.Contact)
              .WithMany(c => c.Phones)
              .HasForeignKey(e => e.ContactId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
