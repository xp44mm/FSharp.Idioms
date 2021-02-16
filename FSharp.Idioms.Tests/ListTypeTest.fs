namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type ListTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getElementType``() =
        let x = typeof<int list> 
        let y = ListType.getElementType x
        Should.equal y typeof<int>

    [<Fact>]
    member this.``getIsEmpty``() =
        let x = [1]
        let isEmpty = ListType.getIsEmpty <| x.GetType()
        let y = isEmpty x
        Should.equal y false

    [<Fact>]
    member this.``getHead``() =
        let x = [1]
        let head = ListType.getHead <| x.GetType()
        let y = head x
        Should.equal y (box 1)

    [<Fact>]
    member this.``getTail``() =
        let x = [1;2]
        let tail = ListType.getTail <| x.GetType()
        let y = tail x
        Should.equal y (box [2])

    [<Fact>]
    member this.``readList``() =
        let x = [1;2]
        let readList = ListType.readList <| x.GetType()
        let ty, values = readList x

        Should.equal ty typeof<int>
        Should.equal values (Array.map box [|1;2|])

    [<Fact>]
    member this.``getOfArray``() =
        let x = box [|1;2|]
        let ofArray = ListType.getOfArray typeof<int list>
        let y = ofArray.Invoke(null, Array.singleton x) :?> int list

        Should.equal y [1;2]


