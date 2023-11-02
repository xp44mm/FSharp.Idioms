namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions

open FSharp.xUnit
open FSharp.Idioms

type PairTest(output: ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``01 - swap test``() =
        let y = Pair.swap(1, 2)
        Should.equal y (2,1)

    [<Fact>]
    member this.``02 - ofApp test``() =
        let y = Pair.ofApp 1 2
        Should.equal y (1,2)

    [<Fact>]
    member this.``03 - revApp test``() =
        let y = Pair.revApp 1 2
        Should.equal y (2,1)
