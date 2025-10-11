using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Sample.FinancialMarket.Infrastructure.Providers;
using System.Security.Claims;

namespace Sample.FinancialMarket.Infrastructure.Tests.Providers
{
    [Trait("Category", "Unit")]
    public class CurrentUserServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public CurrentUserServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void UserId_WhenUserIsAuthenticated_ShouldReturnCorrectUserIdClaim()
        {
            // Arrange
            var expectedUserId = "auth-user-123";
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, expectedUserId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(claimsPrincipal);
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            var currentUserService = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            var actualUserId = currentUserService.UserId;

            // Assert
            actualUserId.Should().Be(expectedUserId);
        }

        [Fact]
        public void UserId_WhenUserIsNotAuthenticated_ShouldReturnNull()
        {
            // Arrange
            var identity = new ClaimsIdentity(); // Unauthenticated identity
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(claimsPrincipal);
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            var currentUserService = new CurrentUserService(_mockHttpContextAccessor.Object);

            // Act
            var actualUserId = currentUserService.UserId;

            // Assert
            actualUserId.Should().BeNull();
        }
    }
}