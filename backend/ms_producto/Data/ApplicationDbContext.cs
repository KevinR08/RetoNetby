using Microsoft.EntityFrameworkCore;
using ms_producto.Models;

namespace ms_producto.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}
