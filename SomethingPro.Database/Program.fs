namespace SomethingPro.Database

open FSharp.Data.Sql.Common

module Provider =
    open System.Data.Sql
    open FSharp.Data.Sql

    [<Literal>]
    let vendor =
        DatabaseProviderTypes.MSSQLSERVER_SSDT

    [<Literal>]
    let ssdtPath =
        __SOURCE_DIRECTORY__ + @"/database.dacpac"

    type Schema = SqlDataProvider<vendor, SsdtPath=ssdtPath>
    
    let createContext (connectionString: string) =
        Schema.GetDataContext(connectionString)