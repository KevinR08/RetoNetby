using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ms_producto;
using ms_producto.Data;
using Microsoft.Extensions.Logging;


namespace ms_producto.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(); 
});
        builder.ConfigureServices(services =>
        {
            Console.WriteLine("Pruebas para ms prod");
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // SQlite en memoria
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:");
            });

            var sp = services.BuildServiceProvider();

            var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.OpenConnection(); 
            db.Database.EnsureCreated();
            services.AddSingleton(db);
        }
        );
    }
}
