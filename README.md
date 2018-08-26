# ShiftScheduler
A sample shifts scheduler app built with Angular, C# with ASP.NET Core and Entity Framework Core.

## Requirements

Install [.NET Core SDK](https://microsoft.com/net/core)

Install SQL Server
    - Windows: SQL Express
    - macOS / Linux: [Docker](https://www.docker.com/get-started) Image
    Pull SQL Image from docker registry
    ```
    sudo docker pull microsoft/mssql-server-linux
    sudo docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=p@$$w0rd@1234' -p 1433:1433 -d microsoft/mssql-server-linux
    ```
    
## Clone this repository

```
git clone https://github.com/ayman4ze/version-controlled-store.git .
```

# To run the project: 
```
dotnet restore
dotnet ef database update
dotnet run 
```