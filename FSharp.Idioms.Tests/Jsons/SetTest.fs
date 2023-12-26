namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type SetTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize set``() =
    //    let x = set [1;2;3]
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "[1,2,3]"

    //[<Fact>]
    //member _.``deserialize set``() =
    //    let x = "[1,2,3]"
    //    let y = deserialize<Set<int>> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y <| set[1;2;3]

    [<Fact>]
    member _.``read set``() =
        let x = set [1;2;3]
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]

    [<Fact>]
    member _.``write set``() =
        let x = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let y = JsonWriterApp.writeDynamic typeof<Set<int>> x
        //output.WriteLine(Render.stringify y)
        Should.equal y (set[1;2;3])
