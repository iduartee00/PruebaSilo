create database webstore
use webstore
go

create table Bitacora
(
	BitacoraId int identity
		primary key,
	VcTabla varchar(25) not null,
	VcOperacion varchar(25) not null,
	DtFecha datetime not null,
	VcUserDb varchar(20) not null
)
go

create table Facturas
(
	FacturaId int identity
		primary key,
	VentaId int not null,
	FlAmout float not null,
	VcUsrRfc varchar(18) not null,
	VcUsrNombre varchar(35) not null,
	DtVenta datetime not null,
	VcMarcaName varchar(35) not null,
	VcProvName varchar(35) not null,
	VcProdName varchar(35) not null,
	FlUnitAmount float not null
)
go

create table Marcas
(
	MarcaId int identity
		primary key,
	VcMarcaName varchar(35) not null,
	VcMarcaStatus varchar(2) not null
)
go

create table Parametros
(
	ParametroId int identity
		primary key,
	VcParamName varchar(35) not null,
	VcParamValue varchar(50) not null,
	VcParamStatus varchar(2) not null
)
go

create table Proveedores
(
	ProveedorId int identity
		primary key,
	VcProvName varchar(35) not null,
	VcProvDesc varchar(50) not null,
	VcProvStatus varchar(2) not null
)
go

create table Productos
(
	ProductoId int identity
		primary key,
	VcProdName varchar(35) not null,
	ProveedorId int not null
		references Proveedores,
	MarcaId int not null
		references Marcas,
	VcProdStatus varchar(2) not null,
	FlUnitAmount float not null,
	FlLoteAmount float,
	IntCantLote int
)
go

create table Inventario
(
	InventarioId int identity
		primary key,
	ProductoId int not null
		references Productos,
	DtIngreso date not null,
	DtCaducidad date,
	VcVendido varchar(1) constraint def_vendido default '0'
)
go

create table Usuarios
(
	UsuarioId int identity
		primary key,
	VcUsrRfc varchar(18) not null
		unique,
	VcUsrNombre varchar(35) not null,
	VcUsrApellido varchar(35) not null,
	Password varbinary(100)
)
go

create table Ventas
(
	VentaId int identity
		primary key,
	FlAmout float not null,
	UsuarioId int not null
		references Usuarios,
	DtVenta datetime not null
)
go

create table RelVentaProductos
(
	RelVentaProductoId int identity
		primary key,
	InventarioId int not null
		references Inventario,
	VentaId int not null
		references Ventas
)
go

CREATE   PROCEDURE ActualizaMonto @VentaId int
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Monto float;
    set @Monto = (select sum(FlUnitAmount)
    from Productos P
        join Inventario I on P.ProductoId = I.ProductoId
        join RelVentaProductos RVP on I.InventarioId = RVP.InventarioId
        join Ventas V on RVP.VentaId = V.VentaId
    where V.VentaId = @VentaId);
    update Ventas set FlAmout = @Monto where VentaId = @VentaId;
END
go

CREATE   PROCEDURE AltaUsuario
    @rfc varchar(35),
    @nombre varchar(35),
    @apellido varchar(35),
    @pass varchar(35)
AS
BEGIN
    SET NOCOUNT ON;
    insert into Usuarios values (@rfc, @nombre, @apellido, ENCRYPTBYPASSPHRASE('PassDelCifrado', @pass));
END
go

CREATE   PROCEDURE PrepareFactura @VentaId int
AS
BEGIN
      SET NOCOUNT ON;

      --DECLARE THE VARIABLES FOR HOLDING DATA.
      DECLARE @FlAmout float,
          @VcUsrRfc VARCHAR(18),
          @VcUsrNombre VARCHAR(35),
          @DtVenta datetime,
          @VcMarcaName VARCHAR(35),
          @VcProvName VARCHAR(35),
          @VcProdName VARCHAR(35),
          @FlUnitAmount float

      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE Registros CURSOR READ_ONLY
      FOR select V.VentaId,
               FlAmout,
               VcUsrRfc,
               VcUsrNombre,
               V.DtVenta,
               VcMarcaName,
               VcProvName,
               VcProdName,
               FlUnitAmount
        from RelVentaProductos
        join Ventas V on RelVentaProductos.VentaId = V.VentaId
        join Inventario I on RelVentaProductos.InventarioId = I.InventarioId
        join Productos P on I.ProductoId = P.ProductoId
        join Marcas M on P.MarcaId = M.MarcaId
        join Proveedores P2 on P.ProveedorId = P2.ProveedorId
        join Usuarios U on V.UsuarioId = U.UsuarioId
        where V.VentaId = @VentaId;

      --delete from Facturas where CONVERT(VARCHAR(10), DtVenta, 111) between CONVERT(VARCHAR(10), getdate(), 111) and CONVERT(VARCHAR(10), getdate() - 7, 111);
      delete from Facturas where 0=0;

      OPEN Registros
      FETCH NEXT FROM Registros INTO @VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount
      WHILE @@FETCH_STATUS = 0
          BEGIN
              insert into Facturas values (@VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount);
              FETCH NEXT FROM Registros INTO @VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount
          END
      CLOSE Registros
      DEALLOCATE Registros
END
go

CREATE   PROCEDURE PrepareFacturas
AS
BEGIN
      SET NOCOUNT ON;

      --DECLARE THE VARIABLES FOR HOLDING DATA.
      DECLARE @FlAmout float,
          @VcUsrRfc VARCHAR(18),
          @VcUsrNombre VARCHAR(35),
          @DtVenta datetime,
          @VcMarcaName VARCHAR(35),
          @VcProvName VARCHAR(35),
          @VcProdName VARCHAR(35),
          @FlUnitAmount float,
          @VentaId int

      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE Registros CURSOR READ_ONLY
      FOR select V.VentaId,
               FlAmout,
               VcUsrRfc,
               VcUsrNombre,
               V.DtVenta,
               VcMarcaName,
               VcProvName,
               VcProdName,
               FlUnitAmount
        from RelVentaProductos
        join Ventas V on RelVentaProductos.VentaId = V.VentaId
        join Inventario I on RelVentaProductos.InventarioId = I.InventarioId
        join Productos P on I.ProductoId = P.ProductoId
        join Marcas M on P.MarcaId = M.MarcaId
        join Proveedores P2 on P.ProveedorId = P2.ProveedorId
        join Usuarios U on V.UsuarioId = U.UsuarioId;
        --where V.VentaId not in (select VentaId from Facturas);

      --delete from Facturas where CONVERT(VARCHAR(10), DtVenta, 111) between CONVERT(VARCHAR(10), getdate(), 111) and CONVERT(VARCHAR(10), getdate() - 7, 111);
      delete from Facturas where 0=0;

      OPEN Registros
      FETCH NEXT FROM Registros INTO @VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount
      WHILE @@FETCH_STATUS = 0
          BEGIN
              insert into Facturas values (@VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount);
              FETCH NEXT FROM Registros INTO @VentaId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount
          END
      CLOSE Registros
      DEALLOCATE Registros
END
go

CREATE   PROCEDURE RegistraBitacora
    @Tabla varchar(20),
    @Operacion varchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    insert into Bitacora values (@Tabla, @Operacion, SYSDATETIME(), current_user);
END
go