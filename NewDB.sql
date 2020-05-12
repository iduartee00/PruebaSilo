create database websys
use websys
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

CREATE TABLE ReporteInv (
	RepInventId INT IDENTITY
		PRIMARY KEY
	LendId INT NOT NULL,
    VcEmpsRfc VARCHAR(18) NOT NULL,
    VcEmpsNombre VARCHAR(35) NOT NULL,
    DtLend DATETIME NOT NULL,
    VcBrandName VARCHAR(35) NOT NULL,
    VcProvName VARCHAR(35) NOT NULL,
    InformDesc VARCHAR(35) NOT NULL
)
GO

CREATE TABLE DevicesRep ( --No se usa todavía
	DevicesRepId INT IDENTITY
		PRIMARY KEY,
	VcDevStatus VARCHAR(2) NOT NULL,
	IntCantLote INT NOT NULL,
	NomEmpleado VARCHAR(35) NOT NULL,
	NomEmpresa VARCHAR(35) NOT NULL
)
GO

create table Brands
(
	BrandId int identity
		primary key,
	VcBrandName varchar(35) not null,
	VcBrandDesc varchar(50)
)
go

create table Parametros			--Dentro del esquema de DB no se usa ¿No se usa para nada en el código?
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
	VcProvDesc varchar(50)
)
go

CREATE TABLE InformTips (
	InformTipId INT IDENTITY
		PRIMARY KEY,
	VcInformNom VARCHAR(20) NOT NULL
)
GO

INSERT INTO InformTips VALUES 
('Hardware'),
('Software'),
('PC'),
('Comunicaciones');

create table Informaticos
(
	InformId int identity
		primary key,
	InformDesc VARCHAR(35) NOT NULL,
	InformTipId INT not null
		REFERENCES InformTips,
	ProveedorId int not null
		references Proveedores,
	BrandId int not null
		references Brands,
	VcInforStatus varchar(2) not null	--¿Se necesita? SI InvQuant = 0, no está disponible
)
go

create table Empleados
(
	EmpleadoId int identity
		primary key,
	VcEmpsNombre varchar(35) not null,
	VcEmpsApellido varchar(35) not null,
	VcEmpsRfc varchar(18) not null unique
)
go

create table Lending --Para prestamos
(
	LendId int identity
		primary key,
	LendCant int NOT NULL,
	EmpleadoId INT
		REFERENCES Empleados,		/*Dependiendo de cómo se necesite el reporte, este atributo debe o no debe de ir
									Si debe ir, la lógica es que por cada folio de préstamo DEBE haber un solo empleado
									Si no debe ir, la lógica es que por cada folio de préstamo PUEDE haber más de un empleado.*/
	DtLend datetime NOT NULL
)
go

create table Inventario
(
	InventarioId int identity
		primary key,
	InformId int not null
		references Informaticos,
	DtIngreso datetime not null,
	InvQuant INT NOT NULL
)
go

create table RelLendInvent
(
	RelLendInformId int identity
		primary key,
	InventarioId int not null
		references Inventario,
	LendId int not null
		references Lending
)
go

CREATE   PROCEDURE AltaEmpleado
    @nombre varchar(35),
    @apellido varchar(35),
	@rfc varchar(35)
AS
BEGIN
    SET NOCOUNT ON;
    insert into Empleados values (@nombre, @apellido, @rfc);
END
go

CREATE   PROCEDURE PrepareReporteI @LendId int
AS
BEGIN
      SET NOCOUNT ON;

      --DECLARE THE VARIABLES FOR HOLDING DATA.
      DECLARE @VcEmpsRfc VARCHAR(18),
          @VcEmpsNombre VARCHAR(35),
          @DtLend datetime,
          @VcBrandName VARCHAR(35),
          @VcProvName VARCHAR(35),
          @InformDesc VARCHAR(35)

      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE Registros CURSOR READ_ONLY
      FOR select V.LendId,
               VcEmpsRfc,
               VcEmpsNombre,
               V.DtLend,
               VcBrandName,
               VcProvName,
               InformDesc
        from RelLendInvent
        join Lending V on RelLendInvent.LendId = V.LendId
        join Inventario I on RelLendInvent.InventarioId = I.InventarioId
        join Informaticos P on I.InformId = P.InformId
        join Brands M on P.BrandId = M.BrandId
        join Proveedores P2 on P.ProveedorId = P2.ProveedorId
        join Empleados U on V.EmpleadoId = U.EmpleadoId
        where V.LendId = @LendId;

      --delete from Facturas where CONVERT(VARCHAR(10), DtVenta, 111) between CONVERT(VARCHAR(10), getdate(), 111) and CONVERT(VARCHAR(10), getdate() - 7, 111);
      delete from ReporteInv where 0=0;

      OPEN Registros
      FETCH NEXT FROM Registros INTO @LendId,@FlAmout,@VcUsrRfc,@VcUsrNombre,@DtVenta,@VcMarcaName,@VcProvName,@VcProdName,@FlUnitAmount
      WHILE @@FETCH_STATUS = 0
          BEGIN
              insert into ReporteInv values (@LendId, @VcEmpsRfc, @VcEmpsNombre, @DtLend, @VcBrandName, @VcProvName, @InformDesc);
              FETCH NEXT FROM Registros INTO @LendId, @VcEmpsRfc, @VcEmpsNombre, @DtLend, @VcBrandName, @VcProvName, @InformDesc
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