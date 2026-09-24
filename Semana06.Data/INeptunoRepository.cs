using Semana06.Data.Models;

namespace Semana06.Data;

public interface INeptunoRepository
{
    Task<List<Producto>> ListarProductosAsync();
    Task<int> CrearProductoAsync(Producto item);
    Task ActualizarProductoAsync(Producto item);
    Task EliminarProductoAsync(int id);

    Task<List<Categoria>> ListarCategoriasAsync();
    Task<int> CrearCategoriaAsync(Categoria item);
    Task ActualizarCategoriaAsync(Categoria item);
    Task EliminarCategoriaAsync(int id);

    Task<List<Proveedor>> ListarProveedoresAsync(string? nombreContacto = null, string? ciudad = null);
    Task<int> CrearProveedorAsync(Proveedor item);
    Task ActualizarProveedorAsync(Proveedor item);
    Task EliminarProveedorAsync(int id);

    Task<List<Pedido>> ListarPedidosAsync();
    Task<int> CrearPedidoAsync(Pedido item);
    Task ActualizarPedidoAsync(Pedido item);
    Task EliminarPedidoAsync(int id);

    Task<List<DetallePedidoReporte>> ReporteDetallesAsync(DateTime desde, DateTime hasta);
}
