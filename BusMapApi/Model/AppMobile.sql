
CREATE TABLE BusStop (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Address NVARCHAR(500) NOT NULL
);

CREATE TABLE Account (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL ,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    NumberPhone NVARCHAR(15) NOT NULL ,
    Password NVARCHAR(255) NOT NULL,
);
INSERT INTO Account (FullName, Email, NumberPhone, Password)
VALUES 
(N'Nguy?n Trung Tín', 'tin@gmail.com', '0987654321', '123456789');


CREATE TABLE ForgotPassword (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
	OTP NVARCHAR(20) NOT NULL 
);


CREATE TABLE Bus (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RouteId INT,
    RouteNo NVARCHAR(50),
    RouteName NVARCHAR(255),
    Address NVARCHAR(255)
);

CREATE TABLE admin (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL ,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    NumberPhone NVARCHAR(15) NOT NULL ,
    Password NVARCHAR(255) NOT NULL,
);
INSERT INTO admin (FullName, Email, NumberPhone, Password)
VALUES 
(N'Nguyễn Trung Tín', 'Trungtin828465@gmail.com', '0987654321', '123456789');

