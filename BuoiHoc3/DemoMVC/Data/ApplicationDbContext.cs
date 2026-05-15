using DemoMVC.Models;
using DemoMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DemoMVC.Models.Product> SimpleProducts { get; set; }
        public DbSet<DemoMVC.Models.Entities.Product> Products { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
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
            modelBuilder.Entity<DemoMVC.Models.Product>()
                .ToTable("Products")
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DemoMVC.Models.Entities.Product>()
                .ToTable("ShopProducts")
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(d => d.UnitPrice)
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
