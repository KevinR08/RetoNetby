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
    [FromQuery] string? tipo,
    [FromQuery] int? productoId
)
{
    var q = _db.Transacciones.AsNoTracking();

    if (inicio is not null) q = q.Where(t => t.Fecha >= inicio);
    if (fin is not null) q = q.Where(t => t.Fecha <= fin);
    if (!string.IsNullOrWhiteSpace(tipo)) q = q.Where(t => t.Tipo == tipo);
    if (productoId.HasValue) q = q.Where(t => t.ProductoId == productoId);

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

    // PUT /api/Transacciones/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Transaccion dto)
    {
        if (id != dto.Id)
            return BadRequest("ID no coincide");

        var original = await _db.Transacciones.FindAsync(id);
        if (original == null)
            return NotFound();

        // Revertir stock anterior
        await _stock.AjustarStockAsync(original.ProductoId, original.Cantidad,
            original.Tipo == "Compra" ? "Venta" : "Compra");

        // Aplicar stock nuevo
        var ok = await _stock.AjustarStockAsync(dto.ProductoId, dto.Cantidad, dto.Tipo);
        if (!ok) return BadRequest("Stock insuficiente o producto inexistente.");

        // Actualizar transacción
        original.ProductoId = dto.ProductoId;
        original.Tipo = dto.Tipo;
        original.Cantidad = dto.Cantidad;
        original.PrecioUnitario = dto.PrecioUnitario;
        original.Fecha = dto.Fecha;
        original.Detalle = dto.Detalle;

        await _db.SaveChangesAsync();
        return Ok(original);
    }

    // DELETE /api/Transacciones/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var transaccion = await _db.Transacciones.FindAsync(id);
        if (transaccion == null)
            return NotFound();

        // Revertir stock
        await _stock.AjustarStockAsync(transaccion.ProductoId, transaccion.Cantidad,
            transaccion.Tipo == "Compra" ? "Venta" : "Compra");

        _db.Transacciones.Remove(transaccion);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
