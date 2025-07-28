using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ms_transaccion.Services;

public class StockService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _productosApiBase;

    public StockService(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _httpFactory = httpFactory;
        // appsettings.json →  "Servicios": { "Productos": "http://productos" }
        _productosApiBase = cfg["Servicios:Productos"] ?? "http://productos:8080";
    }

    /// <summary>
    /// Valida y aplica movimiento de stock en el microservicio Productos.
    /// </summary>
    /// <returns>True si la operación fue exitosa.</returns>
    public async Task<bool> AjustarStockAsync(int productoId, int cantidad, string tipo)
    {
        using var client = _httpFactory.CreateClient("productos");

        // 1) Obtener el producto actual
        var producto = await client.GetFromJsonAsync<ProductoDto>(
            $"{_productosApiBase}/api/Productos/{productoId}");

        if (producto is null) return false;

        // 2) Validación de reglas
        if (tipo == "Venta" && producto.Stock < cantidad) return false;
        if (cantidad <= 0) return false;

        // 3) Ajuste de stock
        producto.Stock += tipo == "Compra" ? cantidad : -cantidad;

        var resp = await client.PutAsJsonAsync(
            $"{_productosApiBase}/api/Productos/{productoId}", producto);

        return resp.IsSuccessStatusCode;
    }

    // DTO mínimo para el microservicio Productos
    private sealed class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
