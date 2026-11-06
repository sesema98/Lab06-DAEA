USE NeptunoDB;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
BEGIN TRY
DECLARE @c int,@v int,@p int,@o int;
EXEC dbo.usp_Categoria_Crear @NombreCategoria=N'Prueba laboratorio',@NuevoID=@c OUTPUT;
EXEC dbo.usp_Categoria_Actualizar @CategoriaID=@c,@NombreCategoria=N'Actualizada';
IF NOT EXISTS(SELECT 1 FROM Categorias WHERE CategoriaID=@c AND NombreCategoria=N'Actualizada') THROW 51000,'Actualizar categoria',1;
EXEC dbo.usp_Proveedor_Crear @CompaniaNombre=N'Prueba',@NombreContacto=N'Contacto prueba',@Ciudad=N'Lima',@NuevoID=@v OUTPUT;
EXEC dbo.usp_Proveedor_Actualizar @ProveedorID=@v,@CompaniaNombre=N'Actualizado',@NombreContacto=N'Contacto prueba',@Ciudad=N'Lima';
IF NOT EXISTS(SELECT 1 FROM Proveedores WHERE ProveedorID=@v AND CompaniaNombre=N'Actualizado') THROW 51000,'Actualizar proveedor',1;
EXEC dbo.usp_Producto_Crear @NombreProducto=N'Prueba',@ProveedorID=@v,@CategoriaID=@c,@NuevoID=@p OUTPUT;
EXEC dbo.usp_Producto_Actualizar @ProductoID=@p,@NombreProducto=N'Actualizado',@ProveedorID=@v,@CategoriaID=@c;
IF NOT EXISTS(SELECT 1 FROM Productos WHERE ProductoID=@p AND NombreProducto=N'Actualizado') THROW 51000,'Actualizar producto',1;
EXEC dbo.usp_Pedido_Crear @FechaPedido='20261105',@NuevoID=@o OUTPUT;
EXEC dbo.usp_Pedido_Actualizar @PedidoID=@o,@FechaPedido='20261105',@Destinatario=N'Actualizado';
IF NOT EXISTS(SELECT 1 FROM Pedidos WHERE PedidoID=@o AND Destinatario=N'Actualizado') THROW 51000,'Actualizar pedido',1;
INSERT DetallePedidos(PedidoID,ProductoID,PrecioUnidad,Cantidad,Descuento) VALUES(@o,@p,10,2,0);
CREATE TABLE #r(PedidoID int,FechaPedido date,Destinatario nvarchar(60),ProductoID int,NombreProducto nvarchar(60),PrecioUnidad decimal(10,2),Cantidad smallint,Descuento decimal(4,2),Total decimal(12,2));
INSERT #r EXEC dbo.usp_DetallePedido_ListarPorFechas '20261105','20261105';
IF NOT EXISTS(SELECT 1 FROM #r WHERE PedidoID=@o AND Total=20) THROW 51000,'Reporte por fechas',1;
EXEC dbo.usp_Pedido_Eliminar @o;
TRUNCATE TABLE #r;
INSERT #r EXEC dbo.usp_DetallePedido_ListarPorFechas '20261105','20261105';
IF EXISTS(SELECT 1 FROM #r WHERE PedidoID=@o) THROW 51000,'Reporte incluye inactivos',1;
EXEC dbo.usp_Producto_Eliminar @p;
EXEC dbo.usp_Proveedor_Eliminar @v;
EXEC dbo.usp_Categoria_Eliminar @c;
IF NOT EXISTS(SELECT 1 FROM Productos WHERE ProductoID=@p AND Activo=0) OR NOT EXISTS(SELECT 1 FROM Proveedores WHERE ProveedorID=@v AND Activo=0) OR NOT EXISTS(SELECT 1 FROM Categorias WHERE CategoriaID=@c AND Activo=0) OR NOT EXISTS(SELECT 1 FROM Pedidos WHERE PedidoID=@o AND Activo=0) THROW 51000,'Baja logica',1;
ROLLBACK;
PRINT 'CORRECTO: CRUD de las cuatro entidades, persistencia de bajas logicas, reporte inclusivo y exclusion de pedidos inactivos. Transaccion revertida.';
END TRY
BEGIN CATCH
IF @@TRANCOUNT>0 ROLLBACK;
THROW;
END CATCH;
