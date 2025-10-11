using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// EF Core configuration for the Exchange entity.
    /// This class defines how the Exchange entity maps to the database table.
    /// </summary>
    public class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
    {
        public void Configure(EntityTypeBuilder<Exchange> builder)
        {
            // --- Table and Schema ---
            builder.ToTable("Exchanges");

            // --- Primary Key ---
            // The primary key is inherited from EntityBase and is configured by convention,
            // but we can be explicit if needed.
            builder.HasKey(e => e.Id);

            // --- Properties ---
            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("The full name of the stock exchange.");

            builder.Property(e => e.Acronym)
                .HasMaxLength(10)
                .IsRequired()
                .HasComment("The common acronym for the stock exchange (e.g., NYSE).");

            builder.Property(e => e.Country)
                .HasMaxLength(50)
                .IsRequired()
                .HasComment("The country where the stock exchange is located.");

            // --- Index ---
            builder.HasIndex(e => e.Acronym)
                .IsUnique();

            // --- EntityBase Properties Configuration ---
            builder.Property(e => e.CreatedAt).IsRequired().HasComment("The UTC date and time the record was created.");
            builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100).HasComment("The user who created the record.");
            builder.Property(e => e.LastModifiedAt).HasComment("The UTC date and time the record was last modified.");
            builder.Property(e => e.LastModifiedBy).HasMaxLength(100).HasComment("The user who last modified the record.");
            builder.Property(e => e.Version).IsRowVersion().HasComment("A version number for optimistic concurrency control.");
            builder.Property(e => e.IsDeleted).HasDefaultValue(false).HasComment("Flag to indicate if the record is soft-deleted.");
        }
    }
}