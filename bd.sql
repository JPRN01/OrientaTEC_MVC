IF DB_ID('OrientaTEC') IS NULL
BEGIN
    CREATE DATABASE OrientaTEC;
END
GO

USE OrientaTEC;
GO

CREATE TABLE Tipo_Actividad (
    ID_TIPO_ACTIVIDAD INT NOT NULL IDENTITY PRIMARY KEY,
    tipo VARCHAR(25) NOT NULL
);
GO

CREATE TABLE Estado_Actividad (
    ID_ESTADO_ACTIVIDAD INT NOT NULL IDENTITY PRIMARY KEY,
    estado VARCHAR(25) NOT NULL
);
GO

CREATE TABLE Estado_Registrado (
    ID_ESTADO_REGISTRADO INT NOT NULL IDENTITY PRIMARY KEY,
    grabacion_url VARCHAR(300) NULL,
    evidencia_url VARCHAR(300) NULL,
    justificacion VARCHAR(300) NULL,
    ID_ESTADO_ACTIVIDAD INT NOT NULL,
    FOREIGN KEY (ID_ESTADO_ACTIVIDAD) REFERENCES Estado_Actividad(ID_ESTADO_ACTIVIDAD)
);
GO

CREATE TABLE Equipo_Guia (
    GENERACION INT NOT NULL PRIMARY KEY
);
GO

CREATE TABLE Plan_Trabajo (
    ID_PLAN INT NOT NULL IDENTITY PRIMARY KEY,
    GENERACION INT NOT NULL,
    FOREIGN KEY (GENERACION) REFERENCES Equipo_Guia(GENERACION)
);
GO

CREATE TABLE Actividad (
    ID_ACTIVIDAD INT NOT NULL IDENTITY PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(500) NULL,
    semana INT NOT NULL,
    fecha_exacta datetime NOT NULL,
    dias_previos_para_anunciar INT NULL,
    dias_para_recordar INT NULL,
    es_virtual BIT NOT NULL,
    reunion_url VARCHAR(300) NULL,
    afiche_url VARCHAR(300) NULL,
    ID_PLAN INT NOT NULL,
    ID_TIPO_ACTIVIDAD INT NOT NULL,
    ID_ESTADO_REGISTRADO INT NOT NULL,
    FOREIGN KEY (ID_PLAN) REFERENCES Plan_Trabajo(ID_PLAN),
    FOREIGN KEY (ID_TIPO_ACTIVIDAD) REFERENCES Tipo_Actividad(ID_TIPO_ACTIVIDAD),
    FOREIGN KEY (ID_ESTADO_REGISTRADO) REFERENCES Estado_Registrado(ID_ESTADO_REGISTRADO)
);
GO

CREATE TABLE Fecha_Recordatorio (
    ID_FECHA_RECORDATORIO INT NOT NULL IDENTITY PRIMARY KEY,
    fecha date NOT NULL,
    ID_ACTIVIDAD INT NOT NULL,
    FOREIGN KEY (ID_ACTIVIDAD) REFERENCES Actividad(ID_ACTIVIDAD)
);
GO

CREATE TABLE Centro_Academico (
    INICIALES VARCHAR(3) NOT NULL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    es_sede_central BIT NOT NULL
);
GO

CREATE TABLE Estudiante (
    CARNE INT NOT NULL PRIMARY KEY,
    nombre1 VARCHAR(25) NOT NULL,
    nombre2 VARCHAR(25) NULL,
    apellido1 VARCHAR(25) NOT NULL,
    apellido2 VARCHAR(25) NOT NULL,
    correo VARCHAR(100) UNIQUE NOT NULL,
    tel_celular INT NOT NULL,
    centro_academico VARCHAR(3) NOT NULL,
    FOREIGN KEY (centro_academico) REFERENCES Centro_Academico(INICIALES)
);
GO

CREATE TABLE Asistente_Administrativa (
    ID_ASISTENTE INT NOT NULL IDENTITY PRIMARY KEY,
    nombre1 VARCHAR(25) NOT NULL,
    nombre2 VARCHAR(25) NULL,
    apellido1 VARCHAR(25) NOT NULL,
    apellido2 VARCHAR(25) NOT NULL,
    correo VARCHAR(100) UNIQUE NOT NULL,
    hashed_password VARCHAR(100) NOT NULL,
    salt_password VARCHAR(100) NOT NULL,
    tel_celular INT NULL,
    centro_academico VARCHAR(3) NOT NULL,
    FOREIGN KEY (centro_academico) REFERENCES Centro_Academico(INICIALES)
);
GO

CREATE TABLE Profesor (
    CENTRO_ACADEMICO VARCHAR(3) NOT NULL,
    NUMERO int NOT NULL,
    nombre1 VARCHAR(25) NOT NULL,
    nombre2 VARCHAR(25) NULL,
    apellido1 VARCHAR(25) NOT NULL,
    apellido2 VARCHAR(25) NOT NULL,
    correo VARCHAR(100) UNIQUE NOT NULL,
    hashed_password VARCHAR(100) NOT NULL,
    salt_password VARCHAR(100) NOT NULL,
    tel_oficina VARCHAR(21) NULL,
    tel_celular INT NULL,
    imagen_url VARCHAR(300) NULL,
    PRIMARY KEY (CENTRO_ACADEMICO, NUMERO),
    FOREIGN KEY (CENTRO_ACADEMICO) REFERENCES Centro_Academico(INICIALES)
);
GO

CREATE TABLE Profesor_X_Equipo_Guia (
    CENTRO_ACADEMICO VARCHAR(3) NOT NULL,
    NUMERO int NOT NULL,
    GENERACION INT NOT NULL,
    es_coordinador BIT NOT NULL,
    esta_activo BIT NOT NULL,
    PRIMARY KEY (CENTRO_ACADEMICO, NUMERO, GENERACION),
    FOREIGN KEY (CENTRO_ACADEMICO, NUMERO) REFERENCES Profesor(CENTRO_ACADEMICO, NUMERO),
    FOREIGN KEY (GENERACION) REFERENCES Equipo_Guia(GENERACION)
);
GO

CREATE TABLE Profesor_X_Equipo_Guia_X_Actividad (
    CENTRO_ACADEMICO VARCHAR(3) NOT NULL,
    NUMERO int NOT NULL,
    GENERACION INT NOT NULL,
    ID_ACTIVIDAD INT NOT NULL,
    PRIMARY KEY (CENTRO_ACADEMICO, NUMERO, GENERACION, ID_ACTIVIDAD),
    FOREIGN KEY (CENTRO_ACADEMICO, NUMERO, GENERACION) REFERENCES Profesor_X_Equipo_Guia(CENTRO_ACADEMICO, NUMERO, GENERACION),
    FOREIGN KEY (ID_ACTIVIDAD) REFERENCES Actividad(ID_ACTIVIDAD)
);
GO

CREATE TABLE Comentario (
    ID_COMENTARIO INT NOT NULL IDENTITY PRIMARY KEY,
    mensaje VARCHAR(300) NOT NULL,
    emision datetime NOT NULL,
    ID_ACTIVIDAD INT NOT NULL,
    CENTRO_ACADEMICO VARCHAR(3) NOT NULL,
    NUMERO int NOT NULL,
    ID_COMENTARIO_PADRE INT NULL,
    FOREIGN KEY (ID_ACTIVIDAD) REFERENCES Actividad(ID_ACTIVIDAD),
    FOREIGN KEY (CENTRO_ACADEMICO, NUMERO) REFERENCES Profesor(CENTRO_ACADEMICO, NUMERO),
    FOREIGN KEY (ID_COMENTARIO_PADRE) REFERENCES Comentario(ID_COMENTARIO)
);
GO
