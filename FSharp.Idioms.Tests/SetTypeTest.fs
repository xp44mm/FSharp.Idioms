namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type SetTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getElementType``() =
        let x = typeof<int list> 
        let y = SetType.getElementType x
        Should.equal y typeof<int>

    [<Fact>]
    member this.``getToArray``() =
        let x = (box (set [1;2]))
        let toArray = SetType.getToArray typeof<Set<int>>
        let y = toArray.Invoke(null,Array.singleton x) :?> int[]
        Should.equal y [|1;2|]

    [<Fact>]
    member this.``getOfArray``() =
        let x = box [|1;2|]
        let ofArray = SetType.getOfArray typeof<Set<int>>
        let y = ofArray.Invoke(null,Array.singleton x) :?> Set<int>
        Should.equal y (set [1;2])

    [<Fact>]
    member this.``readSet``() =
        let x = (box (set [1;2]))
        let readSet = SetType.readSet typeof<Set<int>>
        let ty, st = readSet x

        Should.equal ty typeof<int>
        Should.equal st [|box 1; box 2|]

    [<Fact>]
    member this.``HashSet``() =
        let x = (box (set [1;2]))
        let readSet = SetType.readSet typeof<Set<int>>
        let ty, st = readSet x

        Should.equal ty typeof<int>
        Should.equal st [|box 1; box 2|]


