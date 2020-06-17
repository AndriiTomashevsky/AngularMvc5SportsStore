using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ServerApp.Models
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasMany<Rating>(p => p.Ratings).WithOptional(p => p.Product).WillCascadeOnDelete();
            //for changing default name of foreign key 
            //modelBuilder.Entity<Product>().HasOptional<Supplier>(s => s.Supplier).WithMany().HasForeignKey(p => p.SupplierId);
        }
    }
}