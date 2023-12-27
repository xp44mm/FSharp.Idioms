namespace FSharp.Idioms.Reflection
open FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System.Collections.Generic

type SetTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    //[<Fact>]
    //member this.``getElementType``() =
    //    let x = typeof<int list> 
    //    let y = SetType.getElementType x
    //    Should.equal y typeof<int>

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
    member this.``set type members``() =
        let ty = typeof<Set<int>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        for m in ty.GetMembers() do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``set type ctor``() =
        let ty = typeof<Set<int>>
        let x = [2;1]
        let y =  SetType.ctor ty x
        Should.equal y <| set [1;2]

    [<Fact>]
    member this.``set type empty``() =
        let ty = typeof<Set<int>>
        let y = SetType.empty ty
        let e:Set<int> = set []
        Should.equal y e



    //[<Fact>]
    //member this.``readSet``() =
    //    let x = (box (set [1;2]))
    //    let readSet = SetType.readSet typeof<Set<int>>
    //    let ty, st = readSet x

    //    Should.equal ty typeof<int>
    //    Should.equal st [|box 1; box 2|]

    //[<Fact>]
    //member this.``HashSet``() =
    //    let x = (box (set [1;2]))
    //    let readSet = SetType.readSet typeof<Set<int>>
    //    let ty, st = readSet x

    //    Should.equal ty typeof<int>
    //    Should.equal st [|box 1; box 2|]


