namespace FSharp.Idioms.Reflection
open FSharp.Idioms

open Xunit

open FSharp.xUnit
open System
open System.Collections.Generic

type HashSetTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``HashSet type members``() =
        let ty = typeof<HashSet<int>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        for m in ty.GetMembers() do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``HashSet type ctor``() =
        let ty = typeof<HashSet<int>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        for m in ty.GetMembers() do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``HashSet type Activator``() =
        let ty = typeof<HashSet<int>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        let x = [1;2;3]
        let y = Activator.CreateInstance(ty,x)
        let e = HashSet x
        Should.equal y e



