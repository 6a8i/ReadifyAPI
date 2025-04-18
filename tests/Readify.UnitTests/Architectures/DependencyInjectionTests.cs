using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Readify.CrossCutting.DependencyInjection;
using Moq;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;

namespace Readify.UnitTests.Architectures
{
    public class DependencyInjectionTests
    {
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        private readonly Mock<IConfiguration> _configurationMock;

        public DependencyInjectionTests()
        {
            _serviceCollectionMock = new Mock<IServiceCollection>();
            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public void AddFusionCache_RegistersRedisCache()
        {
            // Arrange
            var redisConnectionString = "localhost:6379";
            _configurationMock.Setup(config => config.GetSection("ConnectionStrings")["redis_connection_string"])
                .Returns(redisConnectionString);

            var services = new ServiceCollection();

            // Act
            services.AddFusionCacheConfigurations(_configurationMock.Object);

            // Assert
            Assert.NotNull(services);

            Assert.Contains(services, s => s.ServiceType.Equals(typeof(IFusionCache)));
            Assert.Contains(services, s => s.ServiceType.Equals(typeof(IFusionCacheProvider)));
            Assert.Contains(services, s => s.ServiceType.Equals(typeof(IFusionCacheSerializer)));
            Assert.Contains(services, s => s.ServiceType.Equals(typeof(IDistributedCache)));
        }

        [Fact]
        public void AddRepositories_RegistersDbContextAndRepositories()
        {
            // Arrange
            var defaultConnectionString = "Server=localhost,1433;Database=ReadifyDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;";
            _configurationMock.Setup(config => config.GetSection("ConnectionStrings")["DefaultConnection"])
                .Returns(defaultConnectionString);

            var services = new ServiceCollection();

            // Act
            services.AddRepositories(_configurationMock.Object);

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify DbContext registration
            var dbContext = serviceProvider.GetService<ReadifyDatabaseContext>();
            Assert.NotNull(dbContext);
            Assert.IsType<ReadifyDatabaseContext>(dbContext);

            // Verify repository registrations
            var registeredServices = services.Select(s => s.ServiceType).ToList();
            var repositoryInterfaces = AppDomain.CurrentDomain
                .Load("Readify.Application")
                .GetTypes()
                .Where(t => t.IsInterface && t.Name.Contains("Repository", StringComparison.InvariantCultureIgnoreCase));

            foreach (var repositoryInterface in repositoryInterfaces)
            {
                Assert.Contains(repositoryInterface, registeredServices);
            }
        }
    }
}
