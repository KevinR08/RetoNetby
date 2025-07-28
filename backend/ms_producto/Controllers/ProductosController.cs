using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ms_producto.Data;
using ms_producto.Models;

namespace ms_producto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;
    public ProductosController(ApplicationDbContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> Get() =>
        await _ctx.Productos.AsNoTracking().ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> Get(int id)
    {
        var prod = await _ctx.Productos.FindAsync(id);
        return prod is null ? NotFound() : Ok(prod);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Producto dto)
    {
        _ctx.Productos.Add(dto);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Producto dto)
    {
        if (id != dto.Id) return BadRequest();
        _ctx.Entry(dto).State = EntityState.Modified;
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var prod = await _ctx.Productos.FindAsync(id);
        if (prod is null) return NotFound();
        _ctx.Productos.Remove(prod); await _ctx.SaveChangesAsync();
        return NoContent();
    }
}
