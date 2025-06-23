# EventEaseP1

## Overview
EventEaseP1 is an ASP.NET Core MVC web application for event and venue management, featuring advanced filtering, Azure integration, and cloud-ready architecture.

## Features
- Event creation, editing, and deletion
- Venue management with availability status
- Advanced filtering by Event Type, date range, and venue availability
- Image upload for events
- SQL Server and Azure SQL compatibility

## Database Connection Strings

**Local SQL Server (used for demonstration and testing):
**Azure SQL Database (for reference, not currently accessible):*Server=lab7L95SR\SQLEXPRESS;Initial Catalog=POEPART1;Integrated Security=True;Trust Server Certificate=True;*


**Azure SQL Database (for reference, not currently accessible):*"Server=tcp:registrationserver11.database.windows.net,1433;Initial Catalog=PoeP1;Persist Security Info=False;User ID=ST10445678;Password=K3amog3tsw3!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;*


> **Note:**  
> Due to the Azure subscription being disabled and set to read-only, the application is configured to use the local SQL Server database for all testing, demonstration, and video evidence. The Azure SQL Database connection string is included here for reference and future deployment, but cannot be used until the subscription is re-enabled.

## How to Run Locally
1. Clone the repository.
2. Ensure SQL Server Express is running and the local connection string is set in `appsettings.json`.
3. Run the application using Visual Studio or `dotnet run`.

## Links
- [GitHub Repository](https://github.com/HlahaneKea/EventEaseP1)
- [Video Demonstration](https://youtu.be/O_Q6vjtMWXQ?si=_WdE6VaFpGEfmL_S))
- [Azure Web App ](https://eventseasep1-adgxhrevhvfpbqgu.southafricanorth-01.azurewebsites.net/)


## Author
- **Full Name:** Keamogetswe Hlahane
- **Student Number:** ST10445678
- **Module:** CLDV6211 Cloud Development A
