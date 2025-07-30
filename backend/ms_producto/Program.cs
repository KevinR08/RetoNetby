using ms_producto.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)    
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();             
    });
});

var app = builder.Build();

app.UseRouting(); 
app.UseCors("AllowFrontend");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}




// Endpoints
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("📦 Base de datos no existe. Creando y migrando...");
            db.Database.Migrate();
        }
        else
        {
            Console.WriteLine("✅ Base de datos ya existe. Aplicando migraciones pendientes (si hay)...");
            if (db.Database.GetPendingMigrations().Any())
                db.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migración productos: {ex.Message}");
    }
}

app.Run();

public partial class Program { }