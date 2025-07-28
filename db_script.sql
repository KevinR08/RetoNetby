CREATE DATABASE GestionInventario;
GO
USE GestionInventario;
GO
CREATE TABLE Producto(
  Id INT IDENTITY PRIMARY KEY,
  Nombre NVARCHAR(100) NOT NULL,
  Descripcion NVARCHAR(300),
  Categoria NVARCHAR(50),
  ImagenUrl NVARCHAR(200),
  Precio DECIMAL(18,2),
  Stock INT
);
CREATE TABLE Transaccion(
  Id INT IDENTITY PRIMARY KEY,
  Fecha DATETIME2 NOT NULL,
  Tipo VARCHAR(10) NOT NULL, 
  ProductoId INT FOREIGN KEY REFERENCES Producto(Id),
  Cantidad INT,
  PrecioUnitario DECIMAL(18,2),
  PrecioTotal  AS (Cantidad*PrecioUnitario) PERSISTED,
  Detalle NVARCHAR(500)
);
