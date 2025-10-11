using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Infrastructure.Persistence;
using System.Net;
using System.Net.Http.Json;

namespace Sample.FinancialMarket.Api.Tests.Controllers
{
    /// <summary>
    /// End-to-End tests for the Stocks API endpoints.
    /// These tests use WebApplicationFactory to host the API in-memory and test the full request pipeline.
    /// </summary>
    [Trait("Category", "EndToEnd")]
    public class StocksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public StocksControllerTests(WebApplicationFactory<Program> factory)
        {
            // For each test run, we configure the factory to use a fresh in-memory database.
            // This isolates the tests from each other and from real databases.
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // 1. Remove the real DbContext configuration that points to SQL Server.
                    services.RemoveAll(typeof(DbContextOptions<FinanceDbContext>));

                    // 2. Add an in-memory database provider for EF Core.
                    services.AddDbContext<FinanceDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_Stocks");
                    });

                    // We could also replace other external dependencies here, like Marten or Redis,
                    // if they were not needed for this specific test.
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateStock_WithValidData_ShouldReturnCreatedAndPersistStockInDatabase()
        {
            // --- Arrange ---
            // 1. Define the data for the HTTP POST request.
            var createStockRequest = new CreateStockRequest("NVDA", "Nvidia Corporation", "Technology", 500, 1_200_000_000_000m);

            // --- Act ---
            // 2. Make a real HTTP POST request to the in-memory running API.
            var response = await _client.PostAsJsonAsync("/api/v1/stocks", createStockRequest);

            // --- Assert (Phase 1: API Response) ---
            // 3. Verify that the HTTP response is correct.
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var apiResult = await response.Content.ReadFromJsonAsync<Foundry.Application.Abstractions.Responses.Result<StockDto>>();
            apiResult.Should().NotBeNull();
            apiResult!.IsSuccess.Should().Be(true);
            apiResult.Value!.Ticker.Should().Be("NVDA");
            var createdStockId = apiResult.Value.Id;

            // --- Assert (Phase 2: Database State) ---
            // 4. Verify that the data was actually saved to the database correctly.
            //    This is the most important part of an End-to-End test.
            //    We create a new DI scope to get a fresh DbContext instance and check the data directly.
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
            var savedStock = await dbContext.Stocks.FindAsync(createdStockId);

            savedStock.Should().NotBeNull();
            savedStock!.CompanyName.Should().Be("Nvidia Corporation");
        }

        [Fact]
        public async Task CreateStock_WithInvalidData_ShouldReturnBadRequest()
        {
            // --- Arrange ---
            var createStockRequest = new CreateStockRequest("", "", "Technology", 0, 0); // Invalid ticker

            // --- Act ---
            var response = await _client.PostAsJsonAsync("/api/v1/stocks", createStockRequest);

            // --- Assert ---
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}