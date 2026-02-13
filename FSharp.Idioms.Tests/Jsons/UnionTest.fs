namespace FSharp.Idioms.Jsons

open Xunit

open FSharp.xUnit

type UionExample =
| Zero
| OnlyOne of int
| Pair of int * string

type UnionTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``demo``() =
    //    let x = [Zero;OnlyOne 1;Pair(2,"b")]
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "[\"Zero\",{\"OnlyOne\":1},{\"Pair\":[2,\"b\"]}]"

    //[<Fact>]
    //member _.``serialize zero union case``() =
    //    let x = Zero
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y "\"Zero\""

    //[<Fact>]
    //member _.``deserialize zero union case``() =
    //    let x = "\"Zero\""
    //    let y = deserialize<UionExample> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y Zero

    //[<Fact>]
    //member _.``serialize only-one union case``() =
    //    let x = OnlyOne 1
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """{"OnlyOne":1}"""

    //[<Fact>]
    //member _.``deserialize only-one union case``() =
    //    let x = """{"OnlyOne":1}"""
    //    let y = deserialize<UionExample> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y <| OnlyOne 1

    //[<Fact>]
    //member _.``serialize many params union case``() =
    //    let x = Pair(1,"")
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """{"Pair":[1,""]}"""

    //[<Fact>]
    //member _.``deserialize many params union case``() =
    //    let x = """{"Pair":[1,""]}"""
    //    let y = deserialize<UionExample> x
    //    Should.equal y <| Pair(1,"")

    [<Fact>]
    member _.``read zero union case``() =
        let x = Zero
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.String "Zero"

    [<Fact>]
    member _.``write zero union case``() =
        let x = Json.String "Zero"
        let y = JsonWriterApp.writeDynamic typeof<UionExample> x
        //output.WriteLine(Render.stringify y)
        Should.equal y Zero

    [<Fact>]
    member _.``read only-one union case``() =
        let x = OnlyOne 1
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.String "OnlyOne";Json.Number 1.0]

    [<Fact>]
    member _.``write only-one union case``() =
        let x = Json.Array [Json.String "OnlyOne";Json.Number 1.0]
        let y = JsonWriterApp.writeDynamic typeof<UionExample> x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| OnlyOne 1

    [<Fact>]
    member _.``read many params union case``() =
        let x = Pair(1,"")
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Array [Json.String "Pair";Json.Number 1.0;Json.String ""]

    [<Fact>]
    member _.``write many params union case``() =
        let x = Json.Array [Json.String "Pair";Json.Number 1.0;Json.String ""]
        let y = JsonWriterApp.writeDynamic typeof<UionExample> x
        Should.equal y <| Pair(1,"")
