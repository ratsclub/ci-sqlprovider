namespace Moneyball.Core

open FSharp.Data.Sql.Runtime
open FsToolkit.ErrorHandling
open Giraffe
open Microsoft.Data.SqlClient
open SomethingPro.Infrastructure.Database
open SomethingPro.Database

open Saturn

module Endpoint =
    let createUser dataCtx next ctx =
        text "jasd" next ctx

module Entry =
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
        
        let app = result {
            do! Migration.migrate connectionString
            let dataCtx = Provider.createContext connectionString.ConnectionString

            let app = application {
                url ("http://0.0.0.0:8080")
                use_router (route "/" >=> Endpoint.createUser dataCtx)
                memory_cache
                use_static "public"
                use_gzip
            }
            
            return app 
        }
        
        match app with
        | Ok app ->
            run app
            0
        | Error e ->
            printfn $"%A{e}"
            1
