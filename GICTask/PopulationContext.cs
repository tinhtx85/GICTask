using GICTask.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GICTask
{
    public class PopulationContext: DbContext
    {
        public PopulationContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
        public DbSet<Actual> tblActuals { get; set; }
        public DbSet<Estimate> tblEstimates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actual>().ToTable("Actuals");
            modelBuilder.Entity<Estimate>().ToTable("Estimates");
            modelBuilder.Entity<Actual>(entity =>
            {
                entity.HasKey(e => e.State);
                entity.Property(e => e.ActualPopulation).IsRequired();
                entity.Property(e => e.ActualHouseholds).IsRequired();
            });
            modelBuilder.Entity<Estimate>(entity =>
            {
                entity.HasKey(e => new { e.State, e.Districts });
                entity.Property(e => e.EstimatesHouseholds).IsRequired();
                entity.Property(e => e.EstimatesPopulation).IsRequired();
            });
        }
    }
}
