namespace FSharp.Idioms.Jsons

open Xunit

open FSharp.xUnit

open System.Text.RegularExpressions
open System

type JsonRenderTest(output: ITestOutputHelper) =
    [<Fact>]
    member _.``stringifyNormalJson object``() =
        let x = Json.Object ["name",Json.String "abcdefg"; "age", Json.Number 18.0]
        let y = JsonRender.stringifyNormalJson x
        output.WriteLine(y)
        Should.equal y """{"name":"abcdefg","age":18}"""

    [<Fact>]
    member _.``stringifyNormalJson array``() =
        let x = Json.Array [Json.Number 1.0;Json.Number 2.0;Json.Number 3.0]
        let y = JsonRender.stringifyNormalJson x
        output.WriteLine(y)
        Should.equal y "[1,2,3]"

    [<Fact>]
    member _.``stringifyNormalJson null``() =
        let x = Json.Null
        let y = JsonRender.stringifyNormalJson x
        //output.WriteLine(y)
        Should.equal y "null"

    [<Fact>]
    member _.``stringifyNormalJson false``() =
        let x = Json.False
        let y = JsonRender.stringifyNormalJson x
        //output.WriteLine(y)
        Should.equal y "false"

    [<Fact>]
    member _.``stringifyNormalJson true``() =
        let x = Json.True
        let y = JsonRender.stringifyNormalJson x
        //output.WriteLine(y)
        Should.equal y "true"

    [<Fact>]
    member _.``stringifyNormalJson string``() =
        let x = Json.String "abc"
        let y = JsonRender.stringifyNormalJson x
        //output.WriteLine(y)
        Should.equal y "\"abc\""

    [<Fact>]
    member _.``stringifyNormalJson Number``() =
        let x = Json.Number 1.0
        let y = JsonRender.stringifyNormalJson x
        //output.WriteLine(y)
        Should.equal y "1"

    [<Fact>]
    member _.``stringifyNormalJson Number PI``() =
        let x = Json.Number Math.PI
        let y = JsonRender.stringifyNormalJson x
        output.WriteLine(y)
        Should.equal y "3.141592653589793"


