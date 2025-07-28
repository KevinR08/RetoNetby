using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ms_transaccion.Data;
using ms_transaccion.Models;
using ms_transaccion.Services;

namespace ms_transaccion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly StockService _stock;

    public TransaccionesController(ApplicationDbContext db, StockService stock)
        => (_db, _stock) = (db, stock);

    // GET /api/Transacciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaccion>>> GetAll(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fin,
        [FromQuery] string? tipo)
    {
        var q = _db.Transacciones.AsNoTracking();

        if (inicio is not null) q = q.Where(t => t.Fecha >= inicio);
        if (fin    is not null) q = q.Where(t => t.Fecha <= fin);
        if (!string.IsNullOrWhiteSpace(tipo)) q = q.Where(t => t.Tipo == tipo);

        return Ok(await q.OrderByDescending(t => t.Fecha).ToListAsync());
    }

    // GET /api/Transacciones/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Transaccion>> Get(int id)
        => await _db.Transacciones.FindAsync(id) is { } t ? Ok(t) : NotFound();

    // POST /api/Transacciones
    [HttpPost]
    public async Task<IActionResult> Post(Transaccion dto)
    {
        if (dto.Tipo is not ("Compra" or "Venta"))
            return BadRequest("Tipo debe ser 'Compra' o 'Venta'.");

        var ok = await _stock.AjustarStockAsync(dto.ProductoId, dto.Cantidad, dto.Tipo);
        if (!ok) return BadRequest("Stock insuficiente o producto inexistente.");

        _db.Transacciones.Add(dto);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    // PUT /api/Transacciones/5  (opcional, actualizar detalle)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Transaccion dto)
    {
        if (id != dto.Id) return BadRequest();
        _db.Entry(dto).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/Transacciones/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Transacciones.FindAsync(id);
        if (t is null) return NotFound();
        _db.Transacciones.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
