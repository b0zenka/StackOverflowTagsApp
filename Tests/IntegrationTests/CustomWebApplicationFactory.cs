using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi;

namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            builder.ConfigureServices(services =>
            {
                // remove the app's ApplicationDbContext registration
                var toRemove = services
                   .Where(d =>
                       d.ServiceType == typeof(AppDbContext) ||
                       d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                       d.ServiceType == typeof(DbContextOptions))
                   .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                // add InMemoryDb
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        });
    }
}
