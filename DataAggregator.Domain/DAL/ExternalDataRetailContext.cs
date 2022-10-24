using System.Data.Entity;
using DataAggregator.Domain.Model.ExternalDataRetail.ClassifierExternal;
using DataAggregator.Domain.Model.ExternalDataRetail.external;

namespace DataAggregator.Domain.DAL
{
    public class ExternalDataRetailContext : DbContext
    {
        public DbSet<FTG> FTG { get; set; }
        public DbSet<INNGroup> INNGroup { get; set; }
        public DbSet<ExternalLog> ExternalLog { get; set; }
        public DbSet<Atc> Atc{ get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<ExternalView> ExternalView { get; set; }
        public DbSet<PharmacyDataView> PharmacyDataView{ get; set; }
        public DbSet<PharmacySellingStructure> PharmacySellingStructure { get; set; }
        public DbSet<TradeName> TradeName { get; set; }
    }         
}
