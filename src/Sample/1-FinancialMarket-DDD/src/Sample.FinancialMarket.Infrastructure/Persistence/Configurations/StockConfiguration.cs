using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// EF Core configuration for the Stock entity.
    /// </summary>
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("Stocks")
                .HasComment("Stores individual stock information.");

            // --- Primary Key is defined in the base FinancialAssetConfiguration ---

            // --- Specific Properties for Stock ---
            builder.Property(s => s.CompanyName)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The full legal name of the company.");

            builder.Property(s => s.Sector)
                .HasMaxLength(50)
                .HasComment("The industry sector the stock belongs to.");

            builder.Property(s => s.MarketCap)
                .HasColumnType("decimal(20, 2)")
                .HasComment("The total market value of the company's outstanding shares.");

            // --- Index ---
            // It's good practice to create an index on unique business identifiers like the Ticker.
            builder.HasIndex(s => s.Ticker)
                .IsUnique();
        }
    }
}