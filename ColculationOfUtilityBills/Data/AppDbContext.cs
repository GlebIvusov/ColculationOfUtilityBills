using ColculationOfUtilityBills.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ColculationOfUtilityBills.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<PersonPeriod> PersonPeriods { get; set; }
        public DbSet<ServicesList> ServicesLists { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = DbAccountingForUtilities.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServicesList>()
                .HasOne(r => r.Service).WithMany(s=>s.ServicesLists)
                .HasForeignKey(s => s.IdService);

            modelBuilder.Entity<ServicesList>()
                .HasOne(r => r.PersonPeriod)
                .WithMany(t => t.ServicesLists)
                .HasForeignKey(s => s.IdPersonPeriod);
        }
    }
}
