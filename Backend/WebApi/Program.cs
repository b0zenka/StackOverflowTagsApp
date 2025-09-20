using WebApi.Extensions;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Tags API",
                Version = "v1",
                Description = "API do zarz¹dzania tagami pobranymi z StackOverflow"
            });
        });

        builder.Services.AddCors(opt =>
            opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()));

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");
        app.MapControllers();

        if (!builder.Environment.IsEnvironment("Testing"))
            app.MigrateDatabase();
        
        app.Run();
    }
}
