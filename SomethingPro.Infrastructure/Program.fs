namespace SomethingPro.Infrastructure

open Microsoft.Data.SqlClient
open Microsoft.SqlServer.Dac
open System
open System.Reflection

open DbUp
open FsToolkit.ErrorHandling

module Database =
    module Migration =
        let private buildEngine (connectionString: SqlConnectionStringBuilder) =
            DeployChanges
                .To
                .SqlDatabase(connectionString.ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build()

        let migrate connectionString =
            let engine = buildEngine connectionString

            let result = engine.PerformUpgrade()
            
            if result.Successful then
                Ok()
            else
                Error result.Error

    module Service =
        [<Literal>]
        let dacpacPath = __SOURCE_DIRECTORY__ + "/../SomethingPro.Database/database.dacpac"
        
        let extractDacPac (connectionString: SqlConnectionStringBuilder) appName =
            try
                let ds =
                    DacServices(connectionString.ConnectionString)

                ds.Extract(
                    dacpacPath,
                    connectionString.InitialCatalog,
                    appName,
                    Version(1, 0, 0)
                )

                Ok()
            with
            | exn -> Error exn

module Entry =
    open Database
    
    [<EntryPoint>]
    let main _ =
        let connectionString =
            SqlConnectionStringBuilder(
                DataSource = "localhost",
                InitialCatalog = "model",
                TrustServerCertificate = true,
                Password = "P@ssw0rd",
                UserID = "sa"
            )

        let value = result {
            do! Migration.migrate connectionString 
            do! Service.extractDacPac connectionString "SomethingPro"
        }
            
        match value with
        | Ok() ->
            Console.WriteLine $"Applied migrations and extracted dacpac file to {Service.dacpacPath}."
            0
        | Error exn ->
            Console.WriteLine exn
            1
              
