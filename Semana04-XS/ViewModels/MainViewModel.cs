using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semana06.Data;
using Semana06.Data.Models;

namespace Semana04_XS.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INeptunoRepository _repository;

    public ObservableCollection<Producto> Productos { get; } = new();
    public ObservableCollection<Categoria> Categorias { get; } = new();
    public ObservableCollection<Proveedor> Proveedores { get; } = new();
    public ObservableCollection<Pedido> Pedidos { get; } = new();
    public ObservableCollection<DetallePedidoReporte> Reporte { get; } = new();

    [ObservableProperty] private Producto selectedProducto = new();
    [ObservableProperty] private Categoria selectedCategoria = new();
    [ObservableProperty] private Proveedor selectedProveedor = new();
    [ObservableProperty] private Pedido selectedPedido = new() { FechaPedido = DateTime.Today };
    [ObservableProperty] private string? filtroContacto;
    [ObservableProperty] private string? filtroCiudad;
    [ObservableProperty] private DateTime fechaDesde = DateTime.Today.AddMonths(-1);
    [ObservableProperty] private DateTime fechaHasta = DateTime.Today;
    [ObservableProperty] private string statusMessage = "Listo";
    [ObservableProperty] private bool isBusy;

    public MainViewModel(INeptunoRepository repository) => _repository = repository;

    [RelayCommand]
    private async Task CargarTodoAsync() => await RunAsync(async () =>
    {
        await Reload(Productos, await _repository.ListarProductosAsync());
        await Reload(Categorias, await _repository.ListarCategoriasAsync());
        await Reload(Proveedores, await _repository.ListarProveedoresAsync());
        await Reload(Pedidos, await _repository.ListarPedidosAsync());
        StatusMessage = "Información actualizada";
    });

    [RelayCommand] private void NuevoProducto() => SelectedProducto = new();
    [RelayCommand] private async Task GuardarProductoAsync() => await RunAsync(async () =>
    {
        if (string.IsNullOrWhiteSpace(SelectedProducto.NombreProducto)) throw new InvalidOperationException("El nombre del producto es obligatorio.");
        if (SelectedProducto.ProductoID == 0) await _repository.CrearProductoAsync(SelectedProducto); else await _repository.ActualizarProductoAsync(SelectedProducto);
        await Reload(Productos, await _repository.ListarProductosAsync()); NuevoProducto(); StatusMessage = "Producto guardado";
    });
    [RelayCommand] private async Task EliminarProductoAsync() => await DeleteAsync(SelectedProducto?.ProductoID ?? 0, _repository.EliminarProductoAsync, async () => await Reload(Productos, await _repository.ListarProductosAsync()), "producto");

    [RelayCommand] private void NuevaCategoria() => SelectedCategoria = new();
    [RelayCommand] private async Task GuardarCategoriaAsync() => await RunAsync(async () =>
    {
        if (string.IsNullOrWhiteSpace(SelectedCategoria.NombreCategoria)) throw new InvalidOperationException("El nombre de la categoría es obligatorio.");
        if (SelectedCategoria.CategoriaID == 0) await _repository.CrearCategoriaAsync(SelectedCategoria); else await _repository.ActualizarCategoriaAsync(SelectedCategoria);
        await Reload(Categorias, await _repository.ListarCategoriasAsync()); NuevaCategoria(); StatusMessage = "Categoría guardada";
    });
    [RelayCommand] private async Task EliminarCategoriaAsync() => await DeleteAsync(SelectedCategoria?.CategoriaID ?? 0, _repository.EliminarCategoriaAsync, async () => await Reload(Categorias, await _repository.ListarCategoriasAsync()), "categoría");

    [RelayCommand] private void NuevoProveedor() => SelectedProveedor = new();
    [RelayCommand] private async Task GuardarProveedorAsync() => await RunAsync(async () =>
    {
        if (string.IsNullOrWhiteSpace(SelectedProveedor.CompaniaNombre)) throw new InvalidOperationException("La compañía es obligatoria.");
        if (SelectedProveedor.ProveedorID == 0) await _repository.CrearProveedorAsync(SelectedProveedor); else await _repository.ActualizarProveedorAsync(SelectedProveedor);
        await BuscarProveedoresCoreAsync(); NuevoProveedor(); StatusMessage = "Proveedor guardado";
    });
    [RelayCommand] private async Task EliminarProveedorAsync() => await DeleteAsync(SelectedProveedor?.ProveedorID ?? 0, _repository.EliminarProveedorAsync, BuscarProveedoresCoreAsync, "proveedor");
    [RelayCommand] private async Task BuscarProveedoresAsync() => await RunAsync(BuscarProveedoresCoreAsync);
    private async Task BuscarProveedoresCoreAsync() => await Reload(Proveedores, await _repository.ListarProveedoresAsync(FiltroContacto, FiltroCiudad));

    [RelayCommand] private void NuevoPedido() => SelectedPedido = new() { FechaPedido = DateTime.Today };
    [RelayCommand] private async Task GuardarPedidoAsync() => await RunAsync(async () =>
    {
        if (SelectedPedido.FechaPedido == default) throw new InvalidOperationException("La fecha del pedido es obligatoria.");
        if (SelectedPedido.PedidoID == 0) await _repository.CrearPedidoAsync(SelectedPedido); else await _repository.ActualizarPedidoAsync(SelectedPedido);
        await Reload(Pedidos, await _repository.ListarPedidosAsync()); NuevoPedido(); StatusMessage = "Pedido guardado";
    });
    [RelayCommand] private async Task EliminarPedidoAsync() => await DeleteAsync(SelectedPedido?.PedidoID ?? 0, _repository.EliminarPedidoAsync, async () => await Reload(Pedidos, await _repository.ListarPedidosAsync()), "pedido");

    [RelayCommand] private async Task GenerarReporteAsync() => await RunAsync(async () =>
    {
        if (FechaHasta < FechaDesde) throw new InvalidOperationException("La fecha final no puede ser anterior a la inicial.");
        await Reload(Reporte, await _repository.ReporteDetallesAsync(FechaDesde, FechaHasta));
        StatusMessage = $"Reporte generado: {Reporte.Count} registros";
    });

    private async Task DeleteAsync(int id, Func<int, Task> delete, Func<Task> reload, string entity)
    {
        if (id == 0) { StatusMessage = $"Selecciona un {entity} primero"; return; }
        await RunAsync(async () => { await delete(id); await reload(); StatusMessage = $"{entity} eliminado"; });
    }

    private async Task RunAsync(Func<Task> action)
    {
        IsBusy = true;
        try { await action(); }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    private static Task Reload<T>(ObservableCollection<T> target, IEnumerable<T> values)
    {
        target.Clear(); foreach (var item in values) target.Add(item); return Task.CompletedTask;
    }
}
