# Flight-Storage-API
API for searching and storing flight information
# ✈️ FlightStorageService + FlightClientApp

----

## 🧱 Налаштування бази даних

1. Відкрий **DBeaver** і підключися до свого SQL Server.
2. Відкрий файл  
   `SQLScripts/CreateDatabaseAndProcedures.sql`
3. Виконай увесь скрипт (**Ctrl + Enter**)  
   — він створить:
   - Базу `FlightsDB`
   - Таблицю `Flights`
   - Збережені процедури  
     (`GetFlightByNumber`, `GetFlightsByDate`, тощо)
4. Додай тестові дані, наприклад:
   ```sql
   INSERT INTO Flights (FlightNumber, DepartureAirportCity, ArrivalAirportCity, DepartureDateTime, DurationMinutes)
   VALUES ('PS101','Kyiv','London',GETDATE()+1,180);
