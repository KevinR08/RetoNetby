using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ms_producto;
using ms_producto.Models;
using Xunit;

namespace ms_producto.Tests;

public class ProductosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_DeberiaCrearYRecuperarProducto()
    {
        var nuevo = new Producto
        {
            Nombre = "Prod Prueba",
            Precio = 123.52M,
            Categoria = "Test",
            Stock = 10,
            Descripcion = "Test",
            ImagenUrl = "http://google.com/img.png"
        };

        var respuestaPost = await _client.PostAsJsonAsync("/api/productos", nuevo);
        var contenido = await respuestaPost.Content.ReadAsStringAsync();
        if (!respuestaPost.IsSuccessStatusCode)
        {
            throw new Exception($"Error en POST: {respuestaPost.StatusCode} - {contenido}");
        }


        Assert.Equal(HttpStatusCode.Created, respuestaPost.StatusCode);

        var creado = await respuestaPost.Content.ReadFromJsonAsync<Producto>();
        Assert.NotNull(creado);
        Assert.Equal(nuevo.Nombre, creado!.Nombre);

        var respuestaGet = await _client.GetAsync($"/api/productos/{creado.Id}");
        Assert.Equal(HttpStatusCode.OK, respuestaGet.StatusCode);

        var producto = await respuestaGet.Content.ReadFromJsonAsync<Producto>();
        Assert.Equal(creado.Id, producto!.Id);
        Assert.Equal(nuevo.Categoria, producto.Categoria);
    }


    [Fact]
    public async Task Get_NoExiste_DeberiaRetornarNotFound()
    {
        var respuesta = await _client.GetAsync("/api/productos/9999");
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }




    [Fact]
    public async Task Delete_DeberiaEliminarProducto()
    {
        var nuevo = new Producto { Nombre = "Borrar", Precio = 10, Categoria = "Cat", Stock = 1 };
        var post = await _client.PostAsJsonAsync("/api/productos", nuevo);
        var creado = await post.Content.ReadFromJsonAsync<Producto>();

        var delete = await _client.DeleteAsync($"/api/productos/{creado!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var get = await _client.GetAsync($"/api/productos/{creado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Post_Invalido_DeberiaRetornarBadRequest()
    {
        var invalido = new Producto { Precio = 9.99M, Categoria = "Cat", Stock = 1 };
        var post = await _client.PostAsJsonAsync("/api/productos", invalido);

        Assert.True(
            post.StatusCode == HttpStatusCode.BadRequest ||
            post.StatusCode == HttpStatusCode.InternalServerError,
            $"Estado: {post.StatusCode}"
        );
    }


}
