namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type TripleTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``test first``() =
        let triple = (1,2,3)
        let y = Triple.first triple
        let res = 1
        Should.equal y res

    [<Fact>]
    member this.``test firstTwo``() =
        let triple = (1,2,3)
        let y = Triple.firstTwo triple
        let res = 1,2
        Should.equal y res

    [<Fact>]
    member this.``test last``() =
        let triple = (1,2,3)
        let y = Triple.last triple
        let res = 3
        Should.equal y res

    [<Fact>]
    member this.``test lastTwo``() =
        let triple = (1,2,3)
        let y = Triple.lastTwo triple
        let res = 2,3
        Should.equal y res


