
using Microsoft.EntityFrameworkCore;
using PG.ABBs.Webservices.DiaperSizerService.Models;
using PG.ABBs.Webservices.DiaperSizerService.Settings;

namespace PG.ABBs.Webservices.DiaperSizerService.Context
{
    public class DataContext : DbContext
    {
        private readonly DatabaseSettings _databaseSettings;

        public DataContext(DatabaseSettings settings)
        {
            _databaseSettings = settings;
        }

        public DataContext(DbContextOptions<DataContext> options, DatabaseSettings settings) : base(options)
        {
            _databaseSettings = settings;
        }

        public DbSet<DiaperFitFinder> DiaperFitFinders { get; set; }
        public DbSet<DiaperSize> DiaperSizes { get; set; }
        public DbSet<DiaperSizeAssociation> DiaperSizeAssociations { get; set; }
        public DbSet<MarketInfo> MarketInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");
            modelBuilder.Entity<DiaperFitFinder>(entity =>
            {
                entity.HasKey(e => e.DiaperFitFinderID);
            });

            modelBuilder.Entity<DiaperSize>(entity =>
            {
                entity.HasKey(e => e.DiaperSizeID);
            });

            modelBuilder.Entity<DiaperSizeAssociation>(entity =>
            {
                entity.HasKey(e => e.DiaperSizeAssociationID);
            });

            modelBuilder.Entity<MarketInfo>(entity =>
            {
                entity.HasKey(e => e.MarketInfoID);
            });
        }
    }
}
