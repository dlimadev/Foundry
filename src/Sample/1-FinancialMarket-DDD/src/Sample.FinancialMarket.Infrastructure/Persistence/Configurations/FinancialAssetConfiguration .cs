using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// EF Core configuration for the base FinancialAsset entity.
    /// This is where we configure the inheritance mapping strategy and common properties.
    /// </summary>
    public class FinancialAssetConfiguration : IEntityTypeConfiguration<FinancialAsset>
    {
        public void Configure(EntityTypeBuilder<FinancialAsset> builder)
        {
            // Define the Primary Key ONLY on the base type of the hierarchy.
            builder.HasKey(a => a.Id);

            // Instruct EF Core to use the Table-Per-Concrete-Type (TPC) strategy.
            // This will create separate tables for Stock, Portfolio, and Bond,
            // each with a full set of columns, and no "FinancialAssets" table.
            builder.UseTpcMappingStrategy();

            // Configure common properties inherited by all financial assets
            builder.Property(a => a.Ticker)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Price)
                .HasColumnType("decimal(18, 4)");

            // Configure common properties from EntityBase
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.CreatedBy).IsRequired();
            builder.Property(a => a.Version).IsRowVersion();
            builder.Property(a => a.IsDeleted).HasDefaultValue(false);

            // We can also ignore properties that should not be mapped to the database.
            builder.Ignore(a => a.DomainEvents);
        }
    }
}