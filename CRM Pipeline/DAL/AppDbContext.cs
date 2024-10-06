using CRM_Pipeline.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_Pipeline.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Stages> Stages { get; set; }
        public DbSet<LeadStageHistory> Lead_Stage_History { get; set; }
        public DbSet<Products> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
              .HasMany(u => u.Customers)
              .WithOne(c => c.CreatedByUser)
              .HasForeignKey(c => c.CreatedByUserId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasMany(u => u.SentEmails)
                .WithOne(e => e.SentByUser)
                .HasForeignKey(e => e.SentByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customers table configuration
            builder.Entity<Customers>()
                .HasMany(c => c.Leads)
                .WithOne()
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Customers>()
                .HasMany(c => c.ReceivedEmails)
                .WithOne(e => e.SentToCustomer)
                .HasForeignKey(e => e.SentToCustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Emails table configuration
            builder.Entity<Email>()
                .HasOne(e => e.SentByUser)
                .WithMany(u => u.SentEmails)
                .HasForeignKey(e => e.SentByUserId);

            builder.Entity<Email>()
                .HasOne(e => e.SentToCustomer)
                .WithMany(c => c.ReceivedEmails)
                .HasForeignKey(e => e.SentToCustomerId);

            base.OnModelCreating(builder);
        }
    }
}
