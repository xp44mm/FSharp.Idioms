namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type TupleTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize array``() =
    //    let x = (1,"x")
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """[1,"x"]"""

    //[<Fact>]
    //member _.``deserialize array``() =
    //    let x = """[1,"x"]"""
    //    let y = deserialize<int*string> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y (1,"x")

    [<Fact>]
    member _.``read array``() =
        let x = (1,"x")
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.Number 1.0;Json.String "x"]

    [<Fact>]
    member _.``write array``() =
        let x = Json.Array [Json.Number 1.0;Json.String "x"]
        let y = JsonWriterApp.writeDynamic typeof<int*string> x
        //output.WriteLine(Render.stringify y)
        Should.equal y (1,"x")
