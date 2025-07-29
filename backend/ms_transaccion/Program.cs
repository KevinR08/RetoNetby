using ms_transaccion.Data;
using Microsoft.EntityFrameworkCore;
using ms_transaccion.Services;

var builder = WebApplication.CreateBuilder(args);

//Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("productos");
builder.Services.AddScoped<StockService>();

//  Configuración de CORS
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

// Migraciones
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        var maxIntentos = 5;
        var intentos = 0;
        while (!db.Database.CanConnect() && intentos < maxIntentos)
        {
            Console.WriteLine("🕓 Esperando que la base de datos esté disponible...");
            Thread.Sleep(3000); 
            intentos++;
        }

        if (db.Database.CanConnect())
        {
            if (db.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("📄 Migraciones pendientes detectadas. Aplicando...");
                db.Database.Migrate();
            }
        }
        else
        {
            Console.WriteLine("⚠️ No se pudo conectar a la base de datos después de varios intentos.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error en migración transacciones: {ex.Message}");
    }
}


app.Run();


