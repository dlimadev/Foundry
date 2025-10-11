// The code should be in English
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

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Ticker)
                .HasMaxLength(10)
                .IsRequired()
                .HasComment("The unique ticker symbol for the stock (e.g., MSFT).");

            builder.Property(s => s.CompanyName)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The full legal name of the company.");

            builder.Property(s => s.Sector)
                .HasMaxLength(50)
                .HasComment("The industry sector the stock belongs to.");

            builder.Property(s => s.Price)
                .HasColumnType("decimal(18, 4)")
                .HasComment("The current market price of the stock.");

            builder.Property(s => s.MarketCap)
                .HasColumnType("decimal(20, 2)")
                .HasComment("The total market value of the company's outstanding shares.");

        }
    }
}