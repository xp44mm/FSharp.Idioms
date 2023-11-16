namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System
open System.Collections.Concurrent
open System.Reflection

type ListTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``ModuleType``() =
        let modu = ListType.listModuleType
        for m in modu.GetMembers() do
            output.WriteLine($"{m.Name}:{m.MemberType}")

    [<Fact>]
    member this.``list type members``() =
        let listTypeDef = typeof<list<_>>.GetGenericTypeDefinition()
        for m in listTypeDef.GetMembers(
            BindingFlags.Static ||| BindingFlags.Public
            ) do
            output.WriteLine($"{m.MemberType}, {m.Name}")


    [<Fact>]
    member this.``list type isEmpty``() =
        let listTypeDef = typeof<list<_>>.GetGenericTypeDefinition()
        let get_IsEmpty =
            let memo = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
            fun (ty:Type) -> memo.GetOrAdd(
                ty.GenericTypeArguments.[0],
                ty.GetMethod("get_IsEmpty")
            )
        let isEmpty (value:obj) =
            let mi = get_IsEmpty (value.GetType())
            mi.Invoke(value,[||])
            :?> bool
        let x = [1]
        let y = isEmpty x
        Assert.False(y)

    [<Fact>]
    member this.``isEmpty``() =
        let x = [1]
        //let isEmpty = getIsEmpty <| x.GetType()
        let y = ListType.isEmpty (x.GetType()) x
        Should.equal y false

    [<Fact>]
    member this.``head``() =
        let x = [1]
        let head = ListType.head <| x.GetType()
        let y = head x
        Should.equal y (box 1)

    [<Fact>]
    member this.``tail``() =
        let x = [1;2]
        let tail = ListType.tail <| x.GetType()
        let y = tail x
        Should.equal y (box [2])

    //[<Fact>]
    //member this.``readList``() =
    //    let x = [1;2]
    //    let readList = ListType.readList <| x.GetType()
    //    let ty, values = readList x

    //    Should.equal ty typeof<int>
    //    Should.equal values (Array.map box [|1;2|])
    //    output.WriteLine($"{values}")

    [<Fact>]
    member this.``getOfArray``() =
        let x = box [|1;2|]
        let ofArray = ListType.ofArray typeof<int list>
        let y = ofArray.Invoke(null, Array.singleton x) :?> int list
        Should.equal y [1;2]




