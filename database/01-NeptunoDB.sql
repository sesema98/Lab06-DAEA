SET NOCOUNT ON;
IF DB_ID(N'NeptunoDB') IS NULL EXEC(N'CREATE DATABASE NeptunoDB');
GO
USE NeptunoDB;
GO

IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
CREATE TABLE dbo.Categorias (CategoriaID INT IDENTITY PRIMARY KEY, NombreCategoria NVARCHAR(30) NOT NULL, Descripcion NVARCHAR(200), Activo BIT NOT NULL CONSTRAINT DF_Categorias_Activo DEFAULT 1);
IF OBJECT_ID(N'dbo.Proveedores', N'U') IS NULL
CREATE TABLE dbo.Proveedores (ProveedorID INT IDENTITY PRIMARY KEY, CompaniaNombre NVARCHAR(60) NOT NULL, NombreContacto NVARCHAR(40), CargoContacto NVARCHAR(40), Direccion NVARCHAR(80), Ciudad NVARCHAR(30), CodigoPostal NVARCHAR(10), Pais NVARCHAR(30), Telefono NVARCHAR(24), Fax NVARCHAR(24), Activo BIT NOT NULL CONSTRAINT DF_Proveedores_Activo DEFAULT 1);
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
CREATE TABLE dbo.Clientes (ClienteID INT IDENTITY PRIMARY KEY, Empresa NVARCHAR(60) NOT NULL, NombreContacto NVARCHAR(40), Ciudad NVARCHAR(30), Pais NVARCHAR(30), Telefono NVARCHAR(24));
IF OBJECT_ID(N'dbo.Empleados', N'U') IS NULL
CREATE TABLE dbo.Empleados (EmpleadoID INT IDENTITY PRIMARY KEY, Nombre NVARCHAR(20) NOT NULL, Apellidos NVARCHAR(30) NOT NULL, Cargo NVARCHAR(40), FechaNacimiento DATE, FechaContratacion DATE, Ciudad NVARCHAR(30), Pais NVARCHAR(30));
IF OBJECT_ID(N'dbo.Transportistas', N'U') IS NULL
CREATE TABLE dbo.Transportistas (TransportistaID INT IDENTITY PRIMARY KEY, CompaniaNombre NVARCHAR(60) NOT NULL, Telefono NVARCHAR(24));
IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
CREATE TABLE dbo.Productos (ProductoID INT IDENTITY PRIMARY KEY, NombreProducto NVARCHAR(60) NOT NULL, ProveedorID INT NULL REFERENCES dbo.Proveedores, CategoriaID INT NULL REFERENCES dbo.Categorias, CantidadPorUnidad NVARCHAR(30), PrecioUnidad DECIMAL(10,2) NOT NULL DEFAULT 0, UnidadesEnExistencia SMALLINT NOT NULL DEFAULT 0, UnidadesEnPedido SMALLINT NOT NULL DEFAULT 0, NivelDeReorden SMALLINT NOT NULL DEFAULT 0, Descontinuado BIT NOT NULL DEFAULT 0, Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT 1);
IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
CREATE TABLE dbo.Pedidos (PedidoID INT IDENTITY PRIMARY KEY, ClienteID INT NULL REFERENCES dbo.Clientes, EmpleadoID INT NULL REFERENCES dbo.Empleados, FechaPedido DATE NOT NULL, FechaRequerida DATE, FechaEnvio DATE, TransportistaID INT NULL REFERENCES dbo.Transportistas, Destinatario NVARCHAR(60), CiudadDestino NVARCHAR(30), PaisDestino NVARCHAR(30), Activo BIT NOT NULL CONSTRAINT DF_Pedidos_Activo DEFAULT 1);
IF OBJECT_ID(N'dbo.DetallePedidos', N'U') IS NULL
CREATE TABLE dbo.DetallePedidos (PedidoID INT NOT NULL REFERENCES dbo.Pedidos, ProductoID INT NOT NULL REFERENCES dbo.Productos, PrecioUnidad DECIMAL(10,2) NOT NULL, Cantidad SMALLINT NOT NULL DEFAULT 1, Descuento DECIMAL(4,2) NOT NULL DEFAULT 0, CONSTRAINT PK_DetallePedidos PRIMARY KEY(PedidoID, ProductoID));
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categorias)
INSERT dbo.Categorias(NombreCategoria, Descripcion) VALUES (N'Bebidas',N'Refrescos, cafés, tés y otras bebidas'),(N'Condimentos',N'Salsas, especias y aderezos'),(N'Confituras',N'Mermeladas, dulces y postres'),(N'Lácteos',N'Quesos y productos lácteos'),(N'Carnes y Embutidos',N'Carnes preparadas y embutidos');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores)
INSERT dbo.Proveedores(CompaniaNombre,NombreContacto,CargoContacto,Direccion,Ciudad,CodigoPostal,Pais,Telefono,Fax) VALUES
(N'Lácteos García S.A.',N'Ana García',N'Gerente de Ventas',N'Av. Los Álamos 245',N'Lima',N'15024',N'Perú',N'511-4567890',N'511-4567891'),
(N'Bebidas del Sur Ltda.',N'Carlos Ramírez',N'Jefe Comercial',N'Jr. Comercio 890',N'Arequipa',N'04001',N'Perú',N'054-223344',N'054-223345');
IF NOT EXISTS (SELECT 1 FROM dbo.Clientes)
INSERT dbo.Clientes(Empresa,NombreContacto,Ciudad,Pais,Telefono) VALUES (N'Comercial Andina SAC',N'Pedro Salazar',N'Lima',N'Perú',N'511-2345678'),(N'Supermercados del Norte',N'Rosa Medina',N'Trujillo',N'Perú',N'044-334455');
IF NOT EXISTS (SELECT 1 FROM dbo.Empleados)
INSERT dbo.Empleados(Nombre,Apellidos,Cargo,FechaNacimiento,FechaContratacion,Ciudad,Pais) VALUES (N'Juan',N'Pérez Gómez',N'Vendedor','1990-05-12','2020-01-15',N'Lima',N'Perú'),(N'María',N'López Díaz',N'Supervisora','1988-09-23','2018-03-01',N'Lima',N'Perú');
IF NOT EXISTS (SELECT 1 FROM dbo.Transportistas)
INSERT dbo.Transportistas(CompaniaNombre,Telefono) VALUES (N'Transportes Rápido SAC',N'511-8889900'),(N'Envíos Seguros EIRL',N'511-7776655');
IF NOT EXISTS (SELECT 1 FROM dbo.Productos)
INSERT dbo.Productos(NombreProducto,ProveedorID,CategoriaID,CantidadPorUnidad,PrecioUnidad,UnidadesEnExistencia,UnidadesEnPedido,NivelDeReorden,Descontinuado) VALUES (N'Café Andino Premium',2,1,N'500 g',45.90,120,30,20,0),(N'Queso Fresco Andino',1,4,N'1 kg',22,60,10,10,0);
IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos)
INSERT dbo.Pedidos(ClienteID,EmpleadoID,FechaPedido,FechaRequerida,FechaEnvio,TransportistaID,Destinatario,CiudadDestino,PaisDestino) VALUES (1,1,'2026-08-10','2026-08-20','2026-08-15',1,N'Comercial Andina SAC',N'Lima',N'Perú'),(2,2,'2026-08-12','2026-08-22','2026-08-18',2,N'Supermercados del Norte',N'Trujillo',N'Perú');
IF NOT EXISTS (SELECT 1 FROM dbo.DetallePedidos)
INSERT dbo.DetallePedidos(PedidoID,ProductoID,PrecioUnidad,Cantidad,Descuento) VALUES (1,1,45.90,10,0),(2,2,22,8,0.10);
GO
