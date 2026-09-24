USE NeptunoDB;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Listar AS SELECT * FROM dbo.Productos WHERE Activo=1 ORDER BY NombreProducto;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Crear @NombreProducto NVARCHAR(60),@ProveedorID INT=NULL,@CategoriaID INT=NULL,@CantidadPorUnidad NVARCHAR(30)=NULL,@PrecioUnidad DECIMAL(10,2)=0,@UnidadesEnExistencia SMALLINT=0,@UnidadesEnPedido SMALLINT=0,@NivelDeReorden SMALLINT=0,@Descontinuado BIT=0,@NuevoID INT OUTPUT AS BEGIN SET NOCOUNT ON; INSERT dbo.Productos(NombreProducto,ProveedorID,CategoriaID,CantidadPorUnidad,PrecioUnidad,UnidadesEnExistencia,UnidadesEnPedido,NivelDeReorden,Descontinuado,Activo) VALUES(@NombreProducto,@ProveedorID,@CategoriaID,@CantidadPorUnidad,@PrecioUnidad,@UnidadesEnExistencia,@UnidadesEnPedido,@NivelDeReorden,@Descontinuado,1); SET @NuevoID=CONVERT(INT,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Actualizar @ProductoID INT,@NombreProducto NVARCHAR(60),@ProveedorID INT=NULL,@CategoriaID INT=NULL,@CantidadPorUnidad NVARCHAR(30)=NULL,@PrecioUnidad DECIMAL(10,2)=0,@UnidadesEnExistencia SMALLINT=0,@UnidadesEnPedido SMALLINT=0,@NivelDeReorden SMALLINT=0,@Descontinuado BIT=0 AS UPDATE dbo.Productos SET NombreProducto=@NombreProducto,ProveedorID=@ProveedorID,CategoriaID=@CategoriaID,CantidadPorUnidad=@CantidadPorUnidad,PrecioUnidad=@PrecioUnidad,UnidadesEnExistencia=@UnidadesEnExistencia,UnidadesEnPedido=@UnidadesEnPedido,NivelDeReorden=@NivelDeReorden,Descontinuado=@Descontinuado WHERE ProductoID=@ProductoID AND Activo=1;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar @ProductoID INT AS UPDATE dbo.Productos SET Activo=0 WHERE ProductoID=@ProductoID;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar AS SELECT * FROM dbo.Categorias WHERE Activo=1 ORDER BY NombreCategoria;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Crear @NombreCategoria NVARCHAR(30),@Descripcion NVARCHAR(200)=NULL,@NuevoID INT OUTPUT AS BEGIN SET NOCOUNT ON; INSERT dbo.Categorias(NombreCategoria,Descripcion,Activo) VALUES(@NombreCategoria,@Descripcion,1); SET @NuevoID=CONVERT(INT,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Actualizar @CategoriaID INT,@NombreCategoria NVARCHAR(30),@Descripcion NVARCHAR(200)=NULL AS UPDATE dbo.Categorias SET NombreCategoria=@NombreCategoria,Descripcion=@Descripcion WHERE CategoriaID=@CategoriaID AND Activo=1;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar @CategoriaID INT AS UPDATE dbo.Categorias SET Activo=0 WHERE CategoriaID=@CategoriaID;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar AS SELECT * FROM dbo.Proveedores WHERE Activo=1 ORDER BY CompaniaNombre;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Buscar @NombreContacto NVARCHAR(40)=NULL,@Ciudad NVARCHAR(30)=NULL AS SELECT * FROM dbo.Proveedores WHERE Activo=1 AND (@NombreContacto IS NULL OR NombreContacto LIKE N'%'+@NombreContacto+N'%') AND (@Ciudad IS NULL OR Ciudad LIKE N'%'+@Ciudad+N'%') ORDER BY CompaniaNombre;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Crear @CompaniaNombre NVARCHAR(60),@NombreContacto NVARCHAR(40)=NULL,@CargoContacto NVARCHAR(40)=NULL,@Direccion NVARCHAR(80)=NULL,@Ciudad NVARCHAR(30)=NULL,@CodigoPostal NVARCHAR(10)=NULL,@Pais NVARCHAR(30)=NULL,@Telefono NVARCHAR(24)=NULL,@Fax NVARCHAR(24)=NULL,@NuevoID INT OUTPUT AS BEGIN SET NOCOUNT ON; INSERT dbo.Proveedores(CompaniaNombre,NombreContacto,CargoContacto,Direccion,Ciudad,CodigoPostal,Pais,Telefono,Fax,Activo) VALUES(@CompaniaNombre,@NombreContacto,@CargoContacto,@Direccion,@Ciudad,@CodigoPostal,@Pais,@Telefono,@Fax,1); SET @NuevoID=CONVERT(INT,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Actualizar @ProveedorID INT,@CompaniaNombre NVARCHAR(60),@NombreContacto NVARCHAR(40)=NULL,@CargoContacto NVARCHAR(40)=NULL,@Direccion NVARCHAR(80)=NULL,@Ciudad NVARCHAR(30)=NULL,@CodigoPostal NVARCHAR(10)=NULL,@Pais NVARCHAR(30)=NULL,@Telefono NVARCHAR(24)=NULL,@Fax NVARCHAR(24)=NULL AS UPDATE dbo.Proveedores SET CompaniaNombre=@CompaniaNombre,NombreContacto=@NombreContacto,CargoContacto=@CargoContacto,Direccion=@Direccion,Ciudad=@Ciudad,CodigoPostal=@CodigoPostal,Pais=@Pais,Telefono=@Telefono,Fax=@Fax WHERE ProveedorID=@ProveedorID AND Activo=1;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar @ProveedorID INT AS UPDATE dbo.Proveedores SET Activo=0 WHERE ProveedorID=@ProveedorID;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Listar AS SELECT * FROM dbo.Pedidos WHERE Activo=1 ORDER BY FechaPedido DESC;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Crear @ClienteID INT=NULL,@EmpleadoID INT=NULL,@FechaPedido DATE,@FechaRequerida DATE=NULL,@FechaEnvio DATE=NULL,@TransportistaID INT=NULL,@Destinatario NVARCHAR(60)=NULL,@CiudadDestino NVARCHAR(30)=NULL,@PaisDestino NVARCHAR(30)=NULL,@NuevoID INT OUTPUT AS BEGIN SET NOCOUNT ON; INSERT dbo.Pedidos(ClienteID,EmpleadoID,FechaPedido,FechaRequerida,FechaEnvio,TransportistaID,Destinatario,CiudadDestino,PaisDestino,Activo) VALUES(@ClienteID,@EmpleadoID,@FechaPedido,@FechaRequerida,@FechaEnvio,@TransportistaID,@Destinatario,@CiudadDestino,@PaisDestino,1); SET @NuevoID=CONVERT(INT,SCOPE_IDENTITY()); END;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Actualizar @PedidoID INT,@ClienteID INT=NULL,@EmpleadoID INT=NULL,@FechaPedido DATE,@FechaRequerida DATE=NULL,@FechaEnvio DATE=NULL,@TransportistaID INT=NULL,@Destinatario NVARCHAR(60)=NULL,@CiudadDestino NVARCHAR(30)=NULL,@PaisDestino NVARCHAR(30)=NULL AS UPDATE dbo.Pedidos SET ClienteID=@ClienteID,EmpleadoID=@EmpleadoID,FechaPedido=@FechaPedido,FechaRequerida=@FechaRequerida,FechaEnvio=@FechaEnvio,TransportistaID=@TransportistaID,Destinatario=@Destinatario,CiudadDestino=@CiudadDestino,PaisDestino=@PaisDestino WHERE PedidoID=@PedidoID AND Activo=1;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar @PedidoID INT AS UPDATE dbo.Pedidos SET Activo=0 WHERE PedidoID=@PedidoID;
GO
CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorFechas @FechaDesde DATE,@FechaHasta DATE AS SELECT d.PedidoID,p.FechaPedido,p.Destinatario,d.ProductoID,pr.NombreProducto,d.PrecioUnidad,d.Cantidad,d.Descuento,CAST(d.PrecioUnidad*d.Cantidad*(1-d.Descuento) AS DECIMAL(12,2)) Total FROM dbo.DetallePedidos d INNER JOIN dbo.Pedidos p ON p.PedidoID=d.PedidoID INNER JOIN dbo.Productos pr ON pr.ProductoID=d.ProductoID WHERE p.Activo=1 AND p.FechaPedido BETWEEN @FechaDesde AND @FechaHasta ORDER BY p.FechaPedido,d.PedidoID;
GO
