// The code should be in English
using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Foundry.Domain.Model.Auditing;
using Foundry.Domain.Model.Events;
using Foundry.Domain.Services;
using Foundry.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Sample.FinancialMarket.Infrastructure.Tests.Persistence
{
    /// <summary>
    /// Integration tests for the UnitOfWork class.
    /// These tests focus on verifying the orchestration logic of the UoW. They use a real (in-memory)
    /// DbContext to ensure the ChangeTracker behaves as expected, while mocking external dependencies
    /// like IMediator and IAuditLogStore to isolate the UoW's behavior.
    /// </summary>
    public class UnitOfWorkTests : IDisposable
    {
        private readonly DbContext _dbContext;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IAuditLogStore> _mockAuditLogStore;
        private readonly UnitOfWork _unitOfWork;

        #region Test-Specific Dummy Entities
        [Cacheable]
        private class CacheableDummyEntity : EntityBase { }

        private class NonCacheableDummyEntity : EntityBase { }

        /// <summary>
        /// A private, in-memory DbContext for testing purposes. It allows us to have a real,
        /// functioning ChangeTracker without hitting a physical database.
        /// </summary>
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
            public DbSet<CacheableDummyEntity> CacheableDummies { get; set; }
            public DbSet<NonCacheableDummyEntity> NonCacheableDummies { get; set; }
        }
        #endregion

        public UnitOfWorkTests()
        {
            // --- Arrange: Setup a REAL in-memory DbContext ---
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new TestDbContext(options);

            // --- Arrange: Mock the other external dependencies ---
            _mockMediator = new Mock<IMediator>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockAuditLogStore = new Mock<IAuditLogStore>();

            // Instantiate the UnitOfWork with the REAL DbContext and mocked dependencies.
            _unitOfWork = new UnitOfWork(
                _dbContext,
                _mockMediator.Object,
                _mockCurrentUserService.Object,
                _mockAuditLogStore.Object);
        }

        [Fact]
        public async Task CompleteAsync_WhenCacheableEntityIsModified_ShouldDispatchEventsAndSaveAuditLogs()
        {
            // Arrange
            var entity = new CacheableDummyEntity();
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync(); // Establish a baseline state in the DB.
            entity.IsDeleted = true; // Modify the entity. The ChangeTracker will detect this.

            // Act
            await _unitOfWork.CompleteAsync();

            // Assert
            // 1. Verify that the audit log store was called to save the generated logs.
            _mockAuditLogStore.Verify(a => a.SaveAsync(It.Is<IReadOnlyCollection<AuditLog>>(logs => logs.Count == 1), It.IsAny<CancellationToken>()), Times.Once);

            // 2. Verify that the mediator was called to publish the cache invalidation event.
            //    We verify that Publish was called with an object of type IDomainEvent,
            //    and we use 'It.Is<T>' to assert that the object passed was specifically of type EntityUpdatedEvent.
            _mockMediator.Verify(m => m.Publish(
                It.Is<IDomainEvent>(e => e is EntityUpdatedEvent),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CompleteAsync_WhenNonCacheableEntityIsAdded_ShouldOnlySaveAuditLogs_And_NotDispatchCacheEvents()
        {
            // Arrange
            var entity = new NonCacheableDummyEntity();
            _dbContext.Add(entity); // The ChangeTracker will detect this as 'Added'.

            // Act
            await _unitOfWork.CompleteAsync();

            // Assert
            // 1. Auditing should still happen for non-cacheable entities.
            _mockAuditLogStore.Verify(a => a.SaveAsync(It.Is<IReadOnlyCollection<AuditLog>>(logs => logs.Count == 1), It.IsAny<CancellationToken>()), Times.Once);

            // 2. Cache invalidation events should NOT be published for this entity.
            _mockMediator.Verify(m => m.Publish(It.IsAny<EntityCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_WhenNoChangesAreTracked_ShouldNotSaveAuditLogsOrDispatchEvents()
        {
            // Arrange: Do nothing. The DbContext is fresh and has no tracked changes.

            // Act
            await _unitOfWork.CompleteAsync();

            // Assert
            // 1. SaveChangesAsync is still called by the UoW, even if there are no changes. It will just return 0.
            //    This is expected behavior.

            // 2. If there are no changes, no audit logs should be generated or saved.
            _mockAuditLogStore.Verify(a => a.SaveAsync(It.IsAny<IReadOnlyCollection<AuditLog>>(), It.IsAny<CancellationToken>()), Times.Never);

            // 3. If there are no changes, no domain events should be dispatched.
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Disposes the in-memory database context after each test to ensure test isolation.
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}