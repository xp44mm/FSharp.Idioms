namespace FSharp.Idioms.Jsons

open Xunit

open FSharp.xUnit

type ListTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize list``() =
    //    let x = [1;2;3]
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "[1,2,3]"

    //[<Fact>]
    //member _.``deserialize list``() =
    //    let x = "[1,2,3]"
    //    let y = deserialize<List<int>> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y [1;2;3]


    [<Fact>]
    member _.``read list``() =
        let x = [1;2;3]
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]

    [<Fact>]
    member _.``write list``() =
        let x = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let y = JsonWriterApp.writeDynamic typeof<List<int>> x
        //output.WriteLine(Render.stringify y)
        Should.equal y [1;2;3]
