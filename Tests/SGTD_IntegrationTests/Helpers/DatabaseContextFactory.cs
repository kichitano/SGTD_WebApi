using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGTD_WebApi.DbModel.Context;

namespace SGTD_UnitTests.Helpers
{
    public static class DatabaseContextFactory
    {
        public static DatabaseContext CreateDbContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase($"TestingDb_{Guid.NewGuid()}")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            var configuration = new ConfigurationBuilder().Build();
            var httpContextAccessor = new HttpContextAccessor();

            return new DatabaseContext(options, configuration, httpContextAccessor);
        }
    }
}
