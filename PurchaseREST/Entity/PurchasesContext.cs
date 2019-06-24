using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PurchaseREST
{
    public partial class PurchasesContext : DbContext
    {
        public PurchasesContext()
        {
        }

        public PurchasesContext(DbContextOptions<PurchasesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Purchases> Purchases { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-FOHF5LU;Database=Purchases;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Purchases>(entity =>
            {
                entity.HasKey(e => e.PurchaseId)
                    .HasName("PK_Purchase");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Details).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Price).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Purchases_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
