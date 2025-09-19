using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class HostExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IStackOverflowService, StackOverflowService>();

        services.AddHttpClient<IStackOverflowService, StackOverflowService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "SOTagsApp/1.0");
        });

        return services;
    }

    public static void MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
