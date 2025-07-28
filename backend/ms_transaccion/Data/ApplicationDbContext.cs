using Microsoft.EntityFrameworkCore;
using ms_transaccion.Models;

namespace ms_transaccion.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
}
