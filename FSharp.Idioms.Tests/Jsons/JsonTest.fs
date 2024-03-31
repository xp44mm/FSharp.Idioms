namespace FSharp.Idioms.Jsons
open FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open FSharp.Idioms.Literal

type JsonTest(output:ITestOutputHelper) =
    [<Fact>]
    member _.``get array element``() =
        let x = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let y = x.[1]
        Should.equal y <| Json.Number 2.0

    [<Fact>]
    member _.``get object field``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = x.["name"]
        Should.equal y <| Json.String "abcdefg"

    [<Fact>]
    member _.``floatValue or getValue``() =
        let x = Json.Number 2.0
        let y = x.getValue()
        Should.equal y 2.0

    [<Fact>]
    member _.``stringText or getText``() =
        let x = Json.String "abcdefg"
        let y = x.getText()
        Should.equal y "abcdefg"

    [<Fact>]
    member _.``elements or getElements``() =
        let elements = [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let x = Json.Array elements
        let y = x.getElements()
        Should.equal y elements

    [<Fact>]
    member _.``hasProperty``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = x.hasProperty "age"
        let z = x.hasProperty "Name"
        Assert.True y
        Assert.False z

    [<Fact>]
    member _.``entries or getEntries``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.getEntries()
        Should.equal y fields

    [<Fact>]
    member _.``addProperty``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.assign(["name",Json.String "abc"]).entries
        output.WriteLine(stringify y)
        let e = ["name",Json.String "abc";"age",Json.Number 18.0]
        Should.equal y e

    [<Fact>]
    member _.``replaceProperty``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.assign(["age", Json.Number 22.0]).entries
        output.WriteLine(stringify y)
        let e = ["name",Json.String "abcdefg";"age",Json.Number 22.0]
        Should.equal y e

    [<Fact>]
    member _.``assign``() =
        let target = Json.Object ["name",Json.String "guxin"; "age", Json.Number 18.0]
        let source = Json.Object ["state",Json.String "single"; "age", Json.Number 22.0] 
        let y = target.assign(source)

        //output.WriteLine(stringify y)
        let e = Json.Object ["name",Json.String "guxin";"age",Json.Number 22.0;"state",Json.String "single"]
        Should.equal y e

    [<Fact>]
    member _.``coalesce``() =
        //先来者赢，后来者补充
        let target = Json.Object ["name",Json.String "guxin"; "age", Json.Number 18.0]
        let source = Json.Object ["state",Json.String "single"; "age", Json.Number 22.0] 
        let y = target.coalesce(source)

        //output.WriteLine(stringify y)
        let e = Json.Object ["name",Json.String "guxin";"age",Json.Number 18.0;"state",Json.String "single"]
        Should.equal y e

    [<Fact>]
    member _.``json boolean value``() =
        Should.equal true Json.True.boolValue
        Should.equal false Json.False.boolValue
        let err = Assert.Throws(fun () -> Json.Null.boolValue |> ignore)
        Should.equal err.Message "only for Json.Boolean"



    

