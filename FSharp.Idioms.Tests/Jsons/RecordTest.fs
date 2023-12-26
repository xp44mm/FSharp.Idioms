namespace FSharp.Idioms.Jsons

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type Person = { name : string; age : int }

type RecordTest(output: ITestOutputHelper) =
    do()
    //[<Fact>]
    //member _.``serialize record``() =
    //    let x = { name = "abcdefg"; age = 18 }
    //    let y = serialize x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y """{"name":"abcdefg","age":18}"""

    //[<Fact>]
    //member _.``deserialize record``() =
    //    let x = """{"age":18,"name":"abcdefg"}"""
    //    let y = deserialize<Person> x
    //    //output.WriteLine(Render.stringify y)
    //    Should.equal y { name = "abcdefg"; age = 18 }
        
    [<Fact>]
    member _.``read record``() =
        let x = { name = "abcdefg"; age = 18 }
        let y = JsonReaderApp.readDynamic (x.GetType()) x
        //output.WriteLine(Render.stringify y)
        Should.equal y <| Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]

    [<Fact>]
    member _.``write record``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = JsonWriterApp.writeDynamic typeof<Person> x
        //output.WriteLine(Render.stringify y)
        Should.equal y { name = "abcdefg"; age = 18 }

    [<Fact>]
    member _.``field items test``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = x.["name"]
        Should.equal y <| Json.String "abcdefg"

        
