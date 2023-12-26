namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open System
open FSharp.xUnit

type DBNullConverterTest(output: ITestOutputHelper) =
    do()
    [<Fact>]
    member _.``null DBNull``() =
        let x = box DBNull.Value
        Assert.NotNull(x)
        Should.equal x DBNull.Value
        Assert.True(DBNull.Value.Equals x)
        
        Assert.True((x=DBNull.Value))
    //[<Fact>]
    //member _.``deserialize DBNull``() =
    //    let x = "null"
    //    let y = deserialize<DBNull> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y <| DBNull.Value

    [<Fact>]
    member _.``read DBNull``() =
        let x = DBNull.Value
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y Json.Null

    [<Fact>]
    member _.``DBNull instantiate``() =
        let x = Json.Null
        let y = JsonWriterApp.writeDynamic typeof<DBNull> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| DBNull.Value

