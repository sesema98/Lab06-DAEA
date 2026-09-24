namespace Semana06.Data.Models;

public class Categoria
{
    public int CategoriaID { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

public class Proveedor
{
    public int ProveedorID { get; set; }
    public string CompaniaNombre { get; set; } = string.Empty;
    public string? NombreContacto { get; set; }
    public string? CargoContacto { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Pais { get; set; }
    public string? Telefono { get; set; }
    public string? Fax { get; set; }
    public bool Activo { get; set; } = true;
}

public class Producto
{
    public int ProductoID { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int? ProveedorID { get; set; }
    public int? CategoriaID { get; set; }
    public string? CantidadPorUnidad { get; set; }
    public decimal PrecioUnidad { get; set; }
    public short UnidadesEnExistencia { get; set; }
    public short UnidadesEnPedido { get; set; }
    public short NivelDeReorden { get; set; }
    public bool Descontinuado { get; set; }
    public bool Activo { get; set; } = true;
}

public class Pedido
{
    public int PedidoID { get; set; }
    public int? ClienteID { get; set; }
    public int? EmpleadoID { get; set; }
    public DateTime FechaPedido { get; set; } = DateTime.Today;
    public DateTime? FechaRequerida { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public int? TransportistaID { get; set; }
    public string? Destinatario { get; set; }
    public string? CiudadDestino { get; set; }
    public string? PaisDestino { get; set; }
    public bool Activo { get; set; } = true;
}

public class DetallePedidoReporte
{
    public int PedidoID { get; set; }
    public DateTime FechaPedido { get; set; }
    public string? Destinatario { get; set; }
    public int ProductoID { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal PrecioUnidad { get; set; }
    public short Cantidad { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
}
