namespace FSharp.Idioms.Jsons
open FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit


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
    member _.``ContainsKey``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = x.ContainsKey "age"
        let z = x.ContainsKey "Name"
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
    member _.``get object fields``() =
        let fields = ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let x = Json.Object fields
        let y = x.fields
        Should.equal y fields



    

