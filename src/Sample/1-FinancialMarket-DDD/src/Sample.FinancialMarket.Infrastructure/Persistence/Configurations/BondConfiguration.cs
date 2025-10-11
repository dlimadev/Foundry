using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// EF Core configuration for the Bond entity.
    /// </summary>
    public class BondConfiguration : IEntityTypeConfiguration<Bond>
    {
        public void Configure(EntityTypeBuilder<Bond> builder)
        {
            builder.ToTable("Bonds")
                .HasComment("Stores fixed-income asset information, such as government or corporate bonds.");


            // --- Specific Properties for Bond ---
            builder.Property(b => b.IssuerName)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The name of the entity that issued the bond.");

            builder.Property(b => b.InterestRate)
                .HasColumnType("decimal(5, 4)")
                .HasComment("The annual interest rate (coupon) of the bond.");

            builder.Property(b => b.MaturityDate)
                .HasComment("The date when the bond matures and the principal is returned.");
        }
    }
}