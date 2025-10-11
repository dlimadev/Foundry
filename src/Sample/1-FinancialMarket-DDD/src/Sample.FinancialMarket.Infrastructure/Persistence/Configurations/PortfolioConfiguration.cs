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
            // --- Table and Schema ---
            builder.ToTable("Portfolios")
                .HasComment("Stores portfolio information, which can contain a collection of other financial assets.");

            // --- Primary Key ---
            builder.HasKey(p => p.Id);

            // --- Properties ---
            builder.Property(p => p.Ticker)
                .HasMaxLength(20)
                .IsRequired()
                .HasComment("The unique ticker symbol for the portfolio.");

            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The user-friendly name of the portfolio.");

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .HasComment("An optional description of the portfolio's strategy or purpose.");

            // Properties from EntityBase
            builder.Property(p => p.CreatedAt).IsRequired().HasComment("The UTC date and time the record was created.");
            builder.Property(p => p.CreatedBy).IsRequired().HasComment("The user who created the record.");
            builder.Property(p => p.Version).IsRowVersion().HasComment("A version number for optimistic concurrency.");
            builder.Property(p => p.IsDeleted).HasDefaultValue(false).HasComment("Flag to indicate if the record is soft-deleted.");

            // --- Relationships ---
            // A Portfolio has many Assets. This configures the one-to-many relationship.
            builder.HasMany(p => p.Assets)
                .WithOne();

            // --- Ignore Domain-Only Properties ---
            // The Price is calculated in the domain and not stored directly in this table.
            builder.Ignore(p => p.Price);
        }
    }
}