using System.Data;
using Microsoft.Data.SqlClient;
using Semana06.Data.Models;

namespace Semana06.Data;

public sealed class NeptunoRepository : INeptunoRepository
{
    private readonly string _connectionString;

    public NeptunoRepository(string connectionString) => _connectionString = connectionString;

    private SqlConnection Connection() => new(_connectionString);

    private static object Db(object? value) => value ?? DBNull.Value;

    private static void Add(SqlCommand cmd, string name, SqlDbType type, object? value, int size = 0)
    {
        var parameter = size == 0 ? cmd.Parameters.Add(name, type) : cmd.Parameters.Add(name, type, size);
        parameter.Value = Db(value);
    }

    private Task<List<T>> QueryDisconnectedAsync<T>(string procedure, Action<SqlCommand>? parameters, Func<DataRow, T> map)
    {
        return Task.Run(() =>
        {
            using var connection = Connection();
            using var command = new SqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure };
            parameters?.Invoke(command);

            using var adapter = new SqlDataAdapter(command);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);

            return dataSet.Tables[0].AsEnumerable().Select(map).ToList();
        });
    }

    private async Task<int> InsertAsync(string procedure, Action<SqlCommand> parameters)
    {
        await using var connection = Connection();
        await using var command = new SqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure };
        parameters(command);
        var newId = command.Parameters.Add("@NuevoID", SqlDbType.Int);
        newId.Direction = ParameterDirection.Output;
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return newId.Value == DBNull.Value ? 0 : Convert.ToInt32(newId.Value);
    }

    private async Task ExecuteAsync(string procedure, Action<SqlCommand> parameters)
    {
        await using var connection = Connection();
        await using var command = new SqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure };
        parameters(command);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public Task<List<Producto>> ListarProductosAsync() => QueryDisconnectedAsync("dbo.usp_Producto_Listar", null, r => new Producto
    {
        ProductoID = r.Field<int>("ProductoID"),
        NombreProducto = r.Field<string>("NombreProducto") ?? string.Empty,
        ProveedorID = r.Field<int?>("ProveedorID"),
        CategoriaID = r.Field<int?>("CategoriaID"),
        CantidadPorUnidad = r.Field<string?>("CantidadPorUnidad"),
        PrecioUnidad = r.Field<decimal>("PrecioUnidad"),
        UnidadesEnExistencia = r.Field<short>("UnidadesEnExistencia"),
        UnidadesEnPedido = r.Field<short>("UnidadesEnPedido"),
        NivelDeReorden = r.Field<short>("NivelDeReorden"),
        Descontinuado = r.Field<bool>("Descontinuado"),
        Activo = r.Field<bool>("Activo")
    });

    public Task<int> CrearProductoAsync(Producto x) => InsertAsync("dbo.usp_Producto_Crear", c => ProductoParams(c, x, false));

    public Task ActualizarProductoAsync(Producto x) => ExecuteAsync("dbo.usp_Producto_Actualizar", c => ProductoParams(c, x, true));

    public Task EliminarProductoAsync(int id) => ExecuteAsync("dbo.usp_Producto_Eliminar", c => Add(c, "@ProductoID", SqlDbType.Int, id));

    private static void ProductoParams(SqlCommand c, Producto x, bool id)
    {
        if (id) Add(c, "@ProductoID", SqlDbType.Int, x.ProductoID);
        Add(c, "@NombreProducto", SqlDbType.NVarChar, x.NombreProducto, 60);
        Add(c, "@ProveedorID", SqlDbType.Int, x.ProveedorID);
        Add(c, "@CategoriaID", SqlDbType.Int, x.CategoriaID);
        Add(c, "@CantidadPorUnidad", SqlDbType.NVarChar, x.CantidadPorUnidad, 30);
        Add(c, "@PrecioUnidad", SqlDbType.Decimal, x.PrecioUnidad);
        Add(c, "@UnidadesEnExistencia", SqlDbType.SmallInt, x.UnidadesEnExistencia);
        Add(c, "@UnidadesEnPedido", SqlDbType.SmallInt, x.UnidadesEnPedido);
        Add(c, "@NivelDeReorden", SqlDbType.SmallInt, x.NivelDeReorden);
        Add(c, "@Descontinuado", SqlDbType.Bit, x.Descontinuado);
    }

    public Task<List<Categoria>> ListarCategoriasAsync() => QueryDisconnectedAsync("dbo.usp_Categoria_Listar", null, r => new Categoria
    {
        CategoriaID = r.Field<int>("CategoriaID"),
        NombreCategoria = r.Field<string>("NombreCategoria") ?? string.Empty,
        Descripcion = r.Field<string?>("Descripcion"),
        Activo = r.Field<bool>("Activo")
    });

    public Task<int> CrearCategoriaAsync(Categoria x) => InsertAsync("dbo.usp_Categoria_Crear", c => CategoriaParams(c, x, false));

    public Task ActualizarCategoriaAsync(Categoria x) => ExecuteAsync("dbo.usp_Categoria_Actualizar", c => CategoriaParams(c, x, true));

    public Task EliminarCategoriaAsync(int id) => ExecuteAsync("dbo.usp_Categoria_Eliminar", c => Add(c, "@CategoriaID", SqlDbType.Int, id));

    private static void CategoriaParams(SqlCommand c, Categoria x, bool id)
    {
        if (id) Add(c, "@CategoriaID", SqlDbType.Int, x.CategoriaID);
        Add(c, "@NombreCategoria", SqlDbType.NVarChar, x.NombreCategoria, 30);
        Add(c, "@Descripcion", SqlDbType.NVarChar, x.Descripcion, 200);
    }

    public Task<List<Proveedor>> ListarProveedoresAsync(string? contacto = null, string? ciudad = null) =>
        QueryDisconnectedAsync("dbo.usp_Proveedor_Buscar", c =>
        {
            Add(c, "@NombreContacto", SqlDbType.NVarChar, string.IsNullOrWhiteSpace(contacto) ? null : contacto, 40);
            Add(c, "@Ciudad", SqlDbType.NVarChar, string.IsNullOrWhiteSpace(ciudad) ? null : ciudad, 30);
        }, MapProveedor);

    private static Proveedor MapProveedor(DataRow r) => new()
    {
        ProveedorID = r.Field<int>("ProveedorID"),
        CompaniaNombre = r.Field<string>("CompaniaNombre") ?? string.Empty,
        Activo = r.Field<bool>("Activo"),
        NombreContacto = r.Field<string?>("NombreContacto"),
        CargoContacto = r.Field<string?>("CargoContacto"),
        Direccion = r.Field<string?>("Direccion"),
        Ciudad = r.Field<string?>("Ciudad"),
        CodigoPostal = r.Field<string?>("CodigoPostal"),
        Pais = r.Field<string?>("Pais"),
        Telefono = r.Field<string?>("Telefono"),
        Fax = r.Field<string?>("Fax")
    };

    public Task<int> CrearProveedorAsync(Proveedor x) => InsertAsync("dbo.usp_Proveedor_Crear", c => ProveedorParams(c, x, false));

    public Task ActualizarProveedorAsync(Proveedor x) => ExecuteAsync("dbo.usp_Proveedor_Actualizar", c => ProveedorParams(c, x, true));

    public Task EliminarProveedorAsync(int id) => ExecuteAsync("dbo.usp_Proveedor_Eliminar", c => Add(c, "@ProveedorID", SqlDbType.Int, id));

    private static void ProveedorParams(SqlCommand c, Proveedor x, bool id)
    {
        if (id) Add(c, "@ProveedorID", SqlDbType.Int, x.ProveedorID);
        Add(c, "@CompaniaNombre", SqlDbType.NVarChar, x.CompaniaNombre, 60);
        Add(c, "@NombreContacto", SqlDbType.NVarChar, x.NombreContacto, 40);
        Add(c, "@CargoContacto", SqlDbType.NVarChar, x.CargoContacto, 40);
        Add(c, "@Direccion", SqlDbType.NVarChar, x.Direccion, 80);
        Add(c, "@Ciudad", SqlDbType.NVarChar, x.Ciudad, 30);
        Add(c, "@CodigoPostal", SqlDbType.NVarChar, x.CodigoPostal, 10);
        Add(c, "@Pais", SqlDbType.NVarChar, x.Pais, 30);
        Add(c, "@Telefono", SqlDbType.NVarChar, x.Telefono, 24);
        Add(c, "@Fax", SqlDbType.NVarChar, x.Fax, 24);
    }

    public Task<List<Pedido>> ListarPedidosAsync() => QueryDisconnectedAsync("dbo.usp_Pedido_Listar", null, r => new Pedido
    {
        PedidoID = r.Field<int>("PedidoID"),
        ClienteID = r.Field<int?>("ClienteID"),
        EmpleadoID = r.Field<int?>("EmpleadoID"),
        FechaPedido = r.Field<DateTime>("FechaPedido"),
        FechaRequerida = r.Field<DateTime?>("FechaRequerida"),
        FechaEnvio = r.Field<DateTime?>("FechaEnvio"),
        TransportistaID = r.Field<int?>("TransportistaID"),
        Destinatario = r.Field<string?>("Destinatario"),
        CiudadDestino = r.Field<string?>("CiudadDestino"),
        PaisDestino = r.Field<string?>("PaisDestino"),
        Activo = r.Field<bool>("Activo")
    });

    public Task<int> CrearPedidoAsync(Pedido x) => InsertAsync("dbo.usp_Pedido_Crear", c => PedidoParams(c, x, false));

    public Task ActualizarPedidoAsync(Pedido x) => ExecuteAsync("dbo.usp_Pedido_Actualizar", c => PedidoParams(c, x, true));

    public Task EliminarPedidoAsync(int id) => ExecuteAsync("dbo.usp_Pedido_Eliminar", c => Add(c, "@PedidoID", SqlDbType.Int, id));

    private static void PedidoParams(SqlCommand c, Pedido x, bool id)
    {
        if (id) Add(c, "@PedidoID", SqlDbType.Int, x.PedidoID);
        Add(c, "@ClienteID", SqlDbType.Int, x.ClienteID);
        Add(c, "@EmpleadoID", SqlDbType.Int, x.EmpleadoID);
        Add(c, "@FechaPedido", SqlDbType.Date, x.FechaPedido);
        Add(c, "@FechaRequerida", SqlDbType.Date, x.FechaRequerida);
        Add(c, "@FechaEnvio", SqlDbType.Date, x.FechaEnvio);
        Add(c, "@TransportistaID", SqlDbType.Int, x.TransportistaID);
        Add(c, "@Destinatario", SqlDbType.NVarChar, x.Destinatario, 60);
        Add(c, "@CiudadDestino", SqlDbType.NVarChar, x.CiudadDestino, 30);
        Add(c, "@PaisDestino", SqlDbType.NVarChar, x.PaisDestino, 30);
    }

    public Task<List<DetallePedidoReporte>> ReporteDetallesAsync(DateTime desde, DateTime hasta) =>
        QueryDisconnectedAsync("dbo.usp_DetallePedido_ListarPorFechas", c =>
        {
            Add(c, "@FechaDesde", SqlDbType.Date, desde);
            Add(c, "@FechaHasta", SqlDbType.Date, hasta);
        }, r => new DetallePedidoReporte
        {
            PedidoID = r.Field<int>("PedidoID"),
            FechaPedido = r.Field<DateTime>("FechaPedido"),
            Destinatario = r.Field<string?>("Destinatario"),
            ProductoID = r.Field<int>("ProductoID"),
            NombreProducto = r.Field<string>("NombreProducto") ?? string.Empty,
            PrecioUnidad = r.Field<decimal>("PrecioUnidad"),
            Cantidad = r.Field<short>("Cantidad"),
            Descuento = r.Field<decimal>("Descuento"),
            Total = r.Field<decimal>("Total")
        });
}
