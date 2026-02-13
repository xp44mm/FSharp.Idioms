namespace FSharp.Idioms
open FSharp.Idioms.Jsons

open Xunit

open System
open FSharp.xUnit

type JsonTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``01 - boolean test``() =
        let x = true
        let y = Json.boolean x
        Should.equal y Json.True

    [<Fact>]
    member this.``02 - tryCapture test``() =
        let ls = [
            Json.Object [
                //"PN", Json.Number 1.6
            ]

            Json.Object [
                "material", Json.String "PPH"
            ]

            Json.Object [
                "PN", Json.Number 0.6
            ]

            ]
        let y1 = ls |> Json.tryCapture "PN"
        Should.equal y1.Value (Json.Number 0.6)

        let y2 = ls |> Json.tryCapture "material"
        Should.equal y2.Value (Json.String "PPH")

        let y3 = ls |> Json.tryCapture "DN"
        Should.equal y3 None

