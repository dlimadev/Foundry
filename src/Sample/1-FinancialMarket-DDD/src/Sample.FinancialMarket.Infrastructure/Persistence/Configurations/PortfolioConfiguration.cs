// The code should be in English
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// EF Core configuration for the Portfolio entity.
    /// </summary>
    public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            // --- Table Definition ---
            builder.ToTable("Portfolios")
                .HasComment("Stores portfolio information, which can contain other financial assets (Composite pattern).");

            // --- Primary Key is defined in the base FinancialAssetConfiguration ---

            // --- Specific Properties for Portfolio ---
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The user-friendly name of the portfolio.");

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .HasComment("An optional description of the portfolio's strategy or purpose.");

            // --- Relationships ---
            // A Portfolio has many Assets. This configures the one-to-many relationship
            // and ensures that EF Core knows how to handle the collection.
            builder.HasMany(p => p.Assets)
                .WithOne();
        }
    }
}