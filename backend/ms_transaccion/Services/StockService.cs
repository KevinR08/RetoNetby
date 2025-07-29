using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ms_transaccion.Services
{
    public class StockService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _productosApiBase;

        public StockService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _productosApiBase = "http://productos:8080";
        }

        public async Task<bool> AjustarStockAsync(int productoId, int cantidad, string tipo)
        {
            var http = _httpFactory.CreateClient();

            // Obtener producto actual
            var producto = await http.GetFromJsonAsync<ProductoDto>($"{_productosApiBase}/api/productos/{productoId}");
            if (producto == null) return false;

            if (tipo == "Venta" && producto.Stock < cantidad)
                return false;

            var nuevoStock = tipo == "Compra"
                ? producto.Stock + cantidad
                : producto.Stock - cantidad;

            // Actualizar producto con el nuevo stock
            var actualizar = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Descripcion = producto.Descripcion,
                ImagenUrl = producto.ImagenUrl,
                Precio = producto.Precio,
                Stock = nuevoStock
            };

            var response = await http.PutAsJsonAsync($"{_productosApiBase}/api/productos/{productoId}", actualizar);
            return response.IsSuccessStatusCode;
        }
    }

    // DTO para producto
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
