namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type TupleTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``preComputeTupleReader``() =
        let x = "",1
        let read = TupleType.preComputeTupleReader typeof<string*int>
        let y = read x
        Should.equal y [|box "";box 1|]

    [<Fact>]
    member this.``getTupleElementTypes``() =
        let tps = TupleType.getTupleElementTypes typeof<string*int>
        let y = [|typeof<string>;typeof<int>|]
        Should.equal y tps

    [<Fact>]
    member this.``readTuple``() =
        let x = "",1
        let read = TupleType.readTuple typeof<string*int>
        let types, values = read x |> Array.unzip
        Should.equal types [|typeof<string>;typeof<int>|]
        Should.equal values [|box "";box 1|]




