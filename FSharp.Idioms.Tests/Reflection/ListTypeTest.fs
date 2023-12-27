namespace FSharp.Idioms.Reflection
open FSharp.Idioms

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
        let ty = typeof<list<int>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        for m in ty.GetMembers() do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``list type Cons``() =
        let ty = typeof<list<int>>
        let y = ListType.Cons ty (box 1, box [2;3])
        Should.equal ty (y.GetType())
        Should.equal y (box [1;2;3])

    [<Fact>]
    member this.``list type isEmpty``() =
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
    member this.``get_IsEmpty``() =
        let x = [1]
        let y = ListType.get_IsEmpty (x.GetType()) x
        Should.equal y false

    [<Fact>]
    member this.``get_Head``() =
        let x = [1]
        let head = ListType.get_Head <| x.GetType()
        let y = head x
        Should.equal y (box 1)

    [<Fact>]
    member this.``get_Tail``() =
        let x = [1;2]
        let tail = ListType.get_Tail <| x.GetType()
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

    [<Fact>]
    member this.``interfaces of list``() =
        let ls = box [1;2]
        let ty = typeof<int list>
        for nf in ty.GetInterfaces() do
            output.WriteLine($"{nf.Name}")
        Assert.NotNull(ty.GetInterface("IEnumerable`1"))

    [<Fact>]
    member this.``fold of list``() =
        let ls = [1;2]
        let revls =
            ls
            |> List.fold(fun ls e -> e::ls) []

        Should.equal revls [2;1]
