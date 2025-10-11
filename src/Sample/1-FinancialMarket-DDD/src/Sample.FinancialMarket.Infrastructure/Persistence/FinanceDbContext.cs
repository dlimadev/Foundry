// The code should be in English
using Foundry.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using System.Reflection;

namespace Sample.FinancialMarket.Infrastructure.Persistence
{
    /// <summary>
    /// The concrete DbContext for our sample application, responsible for state-based (CRUD) persistence
    /// with SQL Server.
    /// It is marked as 'internal' to enforce architectural boundaries and ensure that data access
    /// is performed through the abstractions defined in the Domain and Application layers (e.g., IUnitOfWork).
    /// Its only responsibilities are to define the DbSets and apply entity configurations.
    /// </summary>
    internal class FinanceDbContext : DbContext, IApplicationDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinanceDbContext"/> class.
        /// The constructor is now simple, only receiving the EF Core options. All other dependencies
        /// like auditing and user services have been moved to the UnitOfWork.
        /// </summary>
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        // --- DbSets for CRUD Aggregates ---
        // These are the entities managed by EF Core in the SQL Server database.
        // The 'Order' aggregate is NOT here because it is event-sourced.

        /// <summary>
        /// A DbSet for the base type of the financial asset hierarchy.
        /// This helps EF Core correctly configure the Table-Per-Concrete-Type (TPC) inheritance mapping.
        /// </summary>
        public DbSet<FinancialAsset> FinancialAssets { get; set; }

        /// <summary>
        /// A DbSet for the Portfolio aggregate root.
        /// </summary>
        public DbSet<Portfolio> Portfolios { get; set; }

        /// <summary>
        /// A DbSet for the Stock entity.
        /// </summary>
        public DbSet<Stock> Stocks { get; set; }

        /// <summary>
        /// A DbSet for the Bond entity.
        /// </summary>
        public DbSet<Bond> Bonds { get; set; }

        /// <summary>
        /// A DbSet for the Exchange aggregate root.
        /// </summary>
        public DbSet<Exchange> Exchange { get; set; }


        /// <summary>
        /// Overridden to apply all IEntityTypeConfiguration classes from the current assembly (Infrastructure project).
        /// This keeps the DbContext clean and delegates mapping logic to dedicated configuration classes.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

    }
}