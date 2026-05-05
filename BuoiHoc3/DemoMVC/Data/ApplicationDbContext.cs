using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<DeviceCategory> DeviceCategories { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<ImportReceipt> ImportReceipts { get; set; }
        public DbSet<ImportReceiptDetail> ImportReceiptDetails { get; set; }
        public DbSet<ExportReceipt> ExportReceipts { get; set; }
        public DbSet<ExportReceiptDetail> ExportReceiptDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Device>()
                .Property(d => d.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ImportReceiptDetail>()
                .Property(d => d.ImportPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ExportReceiptDetail>()
                .Property(d => d.ExportPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Faculty)
                .WithMany(f => f.Students)
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
