CREATE DATABASE FlightsDB;
GO

USE FlightsDB;
GO

CREATE TABLE FlightsDb.dbo.Flights (
	FlightNumber nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DepartureDateTime datetime2 NOT NULL,
	DepartureAirportCity nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ArrivalAirportCity nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DurationMinutes int NOT NULL,
	Id int IDENTITY(1,1) NOT NULL,
	CONSTRAINT PK__Flights__2EAE6F51D1ABEF4A PRIMARY KEY (FlightNumber),
	CONSTRAINT UQ_Flights_FlightNumber UNIQUE (FlightNumber)
);
GO

CREATE PROCEDURE AddFlight
    @FlightNumber NVARCHAR(10),
    @DepartureDateTime DATETIME2,
    @DepartureAirportCity NVARCHAR(100),
    @ArrivalAirportCity NVARCHAR(100),
    @DurationMinutes INT
AS
BEGIN
    IF @DepartureDateTime BETWEEN GETDATE() AND DATEADD(DAY, 7, GETDATE())
        INSERT INTO Flights VALUES (@FlightNumber, @DepartureDateTime, @DepartureAirportCity, @ArrivalAirportCity, @DurationMinutes);
END;
GO

CREATE   PROCEDURE CleanupOldFlights
AS
BEGIN
    SET NOCOUNT ON;

    -- Удаляем все рейсы, у которых вылет уже прошёл
    DELETE FROM Flights WHERE DepartureDateTime < GETDATE();

    -- Возвращаем количество удалённых строк
    SELECT @@ROWCOUNT AS DeletedFlights;
END
GO

CREATE PROCEDURE GetFlightByNumber
    @FlightNumber NVARCHAR(10)
AS
BEGIN
    SELECT * FROM Flights WHERE FlightNumber = @FlightNumber;
END;
GO

CREATE PROCEDURE GetFlightBySpecificDate
    @Date DATE
AS
BEGIN
    SELECT * FROM Flights WHERE CAST(DepartureDateTime AS DATE) = @Date;
END;
GO

CREATE   PROCEDURE GetFlightsByArrivalCityAndDate
    @City NVARCHAR(100),
    @Date DATE
AS
BEGIN
    SELECT *
    FROM Flights
    WHERE ArrivalAirportCity = @City
      AND CAST(DepartureDateTime AS DATE) = @Date;
END;
GO

CREATE   PROCEDURE GetFlightsByDepartureCityAndDate
    @City NVARCHAR(100),
    @Date DATE
AS
BEGIN
    SELECT *
    FROM Flights
    WHERE DepartureAirportCity = @City
      AND CAST(DepartureDateTime AS DATE) = @Date;
END;
GO

