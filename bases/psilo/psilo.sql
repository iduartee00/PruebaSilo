CREATE DATABASE psilo;
USE psilo;

CREATE TABLE Usuario (
	us_id INT IDENTITY 
		PRIMARY KEY,
	us_nom VARCHAR(15) NOT NULL,
	us_contr VARCHAR(15) NOT NULL
);

CREATE TABLE Empleado (
	emp_id INT IDENTITY
		PRIMARY KEY,
	emp_nom VARCHAR(20) NOT NULL,
	emp_app	VARCHAR(20) NOT NULL
);

CREATE TABLE Marca (
	marca_id INT IDENTITY
		PRIMARY KEY,
	marca_nom VARCHAR(20) NOT NULL
);

CREATE TABLE TipoArt (
	tpart_id INT IDENTITY
		PRIMARY KEY,
	tpart_nom VARCHAR(11) NOT NULL
)

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
	empr_cp VARCHAR(8),
);

CREATE TABLE Inventario (
	inv_id INT IDENTITY
		PRIMARY KEY,
	art_id INT NOT NULL
		REFERENCES Articulo,
	art_cant INT NOT NULL
);

CREATE TABLE Prestamo (
	prst_id INT IDENTITY
		PRIMARY KEY,
	prst_cant INT NOT NULL,
	prst_fch DATETIME NOT NULL
);
