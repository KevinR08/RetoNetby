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

//documentación con Swagger
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// 4️⃣  Endpoints
app.MapControllers();

// 5️⃣  (Opcional) aplica migraciones al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //db.Database.EnsureCreated();
    db.Database.Migrate();
}

app.Run();


