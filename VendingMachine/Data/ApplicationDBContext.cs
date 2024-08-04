using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security;
using VendingMachine.Models;

namespace VendingMachine.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) :base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
            .HasKey(p => p.ID);

            modelBuilder.Entity<PurchaseTransaction>()
                .HasKey(pt => pt.TransactionID);

            modelBuilder.Entity<Account>()
                .HasKey(a => a.ID);

            modelBuilder.Entity<PurchaseTransaction>()
                .HasOne(pt => pt.Customer)
                .WithMany(c => c.PurchaseTransactions)
                .HasForeignKey(pt => pt.CustomerID);

            modelBuilder.Entity<PurchaseTransaction>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.PurchaseTransactions)
                .HasForeignKey(pt => pt.ProductID);
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Add your seed data here
            modelBuilder.Entity<Account>().HasData(
                new Account { ID = 1, Name = "Admin1", Email = "admin1@gmail.com", Password = "P@ssw0rd", Role = "Admin"},
                new Account { ID = 2, Name = "User1", Email = "user1@gmail.com", Password = "P@ssw0rd", Role = "User" },
                new Account { ID = 3, Name = "User2", Email = "user2@gmail.com", Password = "P@ssw0rd", Role = "User" }
                );
        }

        }
}
