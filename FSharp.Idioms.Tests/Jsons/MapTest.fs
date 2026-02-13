namespace FSharp.Idioms.Jsons

open Xunit

open FSharp.xUnit

type MapTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize map``() =
    //    let x = Map [1,"1";2,"2"]
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """[[1,"1"],[2,"2"]]"""

    //[<Fact>]
    //member _.``deserialize map``() =
    //    let x = """[[1,"1"],[2,"2"]]"""
    //    let y = deserialize<Map<int,string>> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y <| Map.ofList [1,"1";2,"2"]

    [<Fact>]
    member _.``read map``() =
        let x = Map [1,"1";2,"2"]
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y 
        <| Json.Array [Json.Array [Json.Number 1.0;Json.String "1"];Json.Array [Json.Number 2.0;Json.String "2"]]

    [<Fact>]
    member _.``write map``() =
        let x = Json.Array [Json.Array [Json.Number 1.0;Json.String "1"];Json.Array [Json.Number 2.0;Json.String "2"]]
        let y = JsonWriterApp.writeDynamic typeof<Map<int,string>> x
        //output.WriteLine(Render.stringify y)
        Should.equal y (Map [1,"1";2,"2"])


