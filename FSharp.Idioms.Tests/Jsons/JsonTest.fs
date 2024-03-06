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
    member _.``hasProperty``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = x.hasProperty "age"
        let z = x.hasProperty "Name"
        Assert.True y
        Assert.False z

    [<Fact>]
    member _.``floatValue``() =
        let x = Json.Number 2.0
        let y = x.floatValue
        Should.equal y 2.0

    [<Fact>]
    member _.``stringText``() =
        let x = Json.String "abcdefg"
        let y = x.stringText
        Should.equal y "abcdefg"

    [<Fact>]
    member _.``get array elements``() =
        let elements = [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let x = Json.Array elements
        let y = x.elements
        Should.equal y elements

    [<Fact>]
    member _.``get object entries``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.entries
        Should.equal y fields

    [<Fact>]
    member _.``addProperty``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.addProperty("name",Json.String "abc").entries
        output.WriteLine(stringify y)
        //注意有两个name，新name在前
        let e = ["name",Json.String "abc";"name",Json.String "abcdefg";"age",Json.Number 18.0]
        Should.equal y e

    [<Fact>]
    member _.``replaceProperty``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.replaceProperty("name",Json.String "abc").entries
        output.WriteLine(stringify y)
        let e = ["name",Json.String "abc";"age",Json.Number 18.0]
        Should.equal y e







    

