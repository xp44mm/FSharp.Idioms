namespace FSharp.Idioms.Jsons
open FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type TupleTest(output: ITestOutputHelper) =

    [<Fact>]
    member _.``read tuple``() =
        let x = (1,"x")
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.Number 1.0;Json.String "x"]

    [<Fact>]
    member _.``write tuple``() =
        let x = Json.Array [Json.Number 1.0;Json.String "x"]
        let y = JsonWriterApp.writeDynamic typeof<int*string> x
        //output.WriteLine(Render.stringify y)
        Should.equal y (1,"x")

    [<Fact>]
    member _.``cast typed``() =
        let x = Json.Array [Json.Number 1.0;Json.String "x"]
        let y = Json.cast<int*string> x
        //output.WriteLine(Render.stringify y)
        Should.equal y (1,"x")
