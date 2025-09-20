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
                // usuń wszystkie stare rejestracje DbContext
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));
                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                // dodaj InMemoryDb
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        });
    }
}
