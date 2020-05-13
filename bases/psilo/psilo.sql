--Checa bien la presentación de las vistas que nos
--mandaron recientemente para que te des una idea
--de cómo empezar a hacer las vistas
CREATE DATABASE psilo;
USE psilo;

CREATE TABLE Empleado (
	emp_id INT IDENTITY
		PRIMARY KEY,
	emp_nom VARCHAR(20) NOT NULL,
	emp_app	VARCHAR(20) NOT NULL
);

CREATE TABLE Usuario (
	us_id INT IDENTITY 
		PRIMARY KEY,
	us_nom VARCHAR(15) NOT NULL,
	emp_id INT NOT NULL
		REFERENCES Empleado,
	us_contr VARCHAR(15) NOT NULL
);

CREATE TABLE Marca (
	marca_id INT IDENTITY
		PRIMARY KEY,
	marca_nom VARCHAR(20) NOT NULL
);

--De esta entidad no se genera una vista a parte,
--es parte de la vista de Artículo.
CREATE TABLE TipoArt (
	tpart_id INT IDENTITY
		PRIMARY KEY,
	tpart_nom VARCHAR(11) NOT NULL
);

CREATE TABLE Articulo (
	art_id INT IDENTITY
		PRIMARY KEY,
	marca_id INT NOT NULL
		REFERENCES Marca,
	art_nom VARCHAR(30) NOT NULL,
	art_seri VARCHAR(50) NOT NULL,
	tpart_id INT NOT NULL
		REFERENCES TipoArt,
	art_desc VARCHAR(120) NOT NULL
);

CREATE TABLE Empresa (
	empr_id INT IDENTITY
		PRIMARY KEY,
	empr_nom VARCHAR(20) NOT NULL,
	empr_calle VARCHAR(20) NOT NULL,
	empr_num VARCHAR(20) NOT NULL,
	empr_col VARCHAR(29) NOT NULL,
	empr_estado VARCHAR(20),
	empr_ciu VARCHAR(20),
	empr_cp VARCHAR(8)
);

CREATE TABLE Inventario (
	inv_id INT IDENTITY
		PRIMARY KEY,
	art_id INT NOT NULL
		REFERENCES Articulo,
	art_cant INT NOT NULL,
	inv_sts VARCHAR(2) NOT NULL
);

CREATE TABLE Prestamo (
	prst_id INT IDENTITY
		PRIMARY KEY,
	emp_id INT NOT NULL
		REFERENCES Empleado,
	prst_cant INT NOT NULL,
	prst_fch DATETIME NOT NULL
);

CREATE TABLE RelPrstInv (
	relip_id INT IDENTITY
		PRIMARY KEY,
	prst_id INT NOT NULL
		REFERENCES Prestamo,
	inv_id INT NOT NULL
		REFERENCES Inventario
);

CREATE TABLE RelEmprUs (
	releu_id INT IDENTITY
		PRIMARY KEY,
	empr_id INT NOT NULL
		REFERENCES Empresa,
	emp_id INT NOT NULL
		REFERENCES Empleado
)
GO

CREATE PROCEDURE ActzaInv
	@PrstId INT 
	,@Proceso INT
AS	
BEGIN
	
	SET NOCOUNT ON;
END
GO

CREATE PROCEDURE ElbRepte
	@PrstId INT
AS
BEGIN
	
	SET NOCOUNT ON;
END
GO
