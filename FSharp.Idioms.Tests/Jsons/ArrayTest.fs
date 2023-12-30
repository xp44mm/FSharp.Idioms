namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type ArrayTest(output: ITestOutputHelper) =
    [<Fact>]
    member _.``read array``() =
        let x = [|1;2;3|]
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        let e = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        //output.WriteLine(Render.stringify y)
        Should.equal y e

    [<Fact>]
    member _.``write array``() =
        let x = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let e = [|1;2;3|]
        let y = JsonWriterApp.writeDynamic (e.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y e


