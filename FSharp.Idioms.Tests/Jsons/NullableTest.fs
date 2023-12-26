namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open System

type NullableTest(output: ITestOutputHelper) =
    do()
    [<Fact>]
    member _.``serialize nullable``() =
        let x = box ()
        Assert.Null(x)

    //[<Fact>]
    //member _.``serialize nullable null``() =
    //    let x = Nullable ()
    //    let y = serialize x
    //    Should.equal y "null"

    //[<Fact>]
    //member _.``deserialize nullable``() =
    //    let x = "3" 
    //    let y = deserialize<Nullable<int>> x
    //    Should.equal y <| Nullable 3

    //[<Fact>]
    //member _.``deserialize nullable null``() =
    //    let x = "null"
    //    let y = deserialize<Nullable<_>> x
    //    Should.equal y <| Nullable ()

    [<Fact>]
    member _.``read nullable``() =
        let x = Nullable 3
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        Should.equal y <| Json.Number 3.0

    [<Fact>]
    member _.``read nullable null``() =
        let x = Nullable<int>()
        let y = JsonReaderApp.readDynamic typeof<Nullable<int>> x
        Should.equal y Json.Null

    [<Fact>]
    member _.``write nullable``() =
        let x = Json.Number 3.0
        let e = Nullable 3
        let y = JsonWriterApp.writeDynamic typeof<Nullable<int>> x
        Should.equal y e 

    [<Fact>]
    member _.``write nullable null``() =
        let x = Json.Null
        let e:Nullable<int> = Nullable ()
        let y = JsonWriterApp.writeDynamic typeof<Nullable<int>> x

        Should.equal y e

    [<Fact>]
    member _.``nullable equality``() =
        Assert.True(Nullable()=Nullable())
        Assert.True(Nullable 3=Nullable 3)

