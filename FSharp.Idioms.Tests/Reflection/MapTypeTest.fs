namespace FSharp.Idioms.Reflection

open Xunit
open Xunit.Abstractions
open FSharp.Idioms
open FSharp.Idioms.Literal
open FSharp.xUnit
open System.Reflection
open System.Collections.Generic

type MapTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> stringify
        |> output.WriteLine

    [<Fact>]
    member this.``IEnumerable of Map``() =
        let mp = Map [1,"1";2,"2"]
        let ty = typeof<Map<int,string>>
        //for nf in ty.GetInterfaces() do
        //    output.WriteLine($"{nf.Name}")
        let ity = ty.GetInterface("IEnumerable`1")
        Assert.NotNull(ity)
        let ety = ity.GenericTypeArguments |> Array.exactlyOne
        //output.WriteLine($"{stringifyTypeDynamic ety}")
        Should.equal ety typeof<KeyValuePair<int,string>>

    [<Fact>]
    member this.``KeyValuePair``() =
        let mp = Map [1,"1";2,"2"]
        let mty = mp.GetType()
        let kty = typeof<KeyValuePair<_,_>>.GetGenericTypeDefinition().MakeGenericType(mty.GenericTypeArguments)
        let get_Key = kty.GetMethod("get_Key")
        let key (kvp:obj) = get_Key.Invoke(kvp,[||])

        let get_Value = kty.GetMethod("get_Value")
        let value (kvp:obj) = get_Value.Invoke(kvp,[||])

        let arr = 
            mp
            |> IEnumerableType.toArray
            |> Array.map(fun kvp ->
                key kvp,value kvp
            )
        output.WriteLine(Literal.stringify arr)

    [<Fact>]
    member this.``IDictionary of Map``() =
        let mp = Map [1,"1";2,"2"]
        let ty = typeof<Map<int,string>>
        //for i in ty.GetInterfaces() do
        //    output.WriteLine($"{i.Name}")
        let id2ty = ty.GetInterface("IDictionary`2")
        Assert.NotNull(id2ty)
        let kty = id2ty.GenericTypeArguments.[0]
        let vty = id2ty.GenericTypeArguments.[1]

        for i in id2ty.GetInterfaces() do
            output.WriteLine($"{i.Name}")
        
        for m in id2ty.GetMethods() do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``get_Keys of Map``() =
        let ty = typeof<Map<int,string>>
        let dty = ty.GetInterface("IDictionary`2")
        Assert.NotNull(dty)
        let kty = dty.GenericTypeArguments.[0]
        let vty = dty.GenericTypeArguments.[1]
        //for mmb in dty.GetMembers() do
        //    output.WriteLine($"{mmb.Name}")
        let get_Keys = dty.GetMethod("get_Keys")
        Assert.NotNull(get_Keys)
        let ety = get_Keys.ReturnType.GetInterface("IEnumerable`1")
        Assert.NotNull(ety)
        Should.equal ety typeof<IEnumerable<int>>

        let mp = Map [1,"1";2,"2"]
        let keysObj = get_Keys.Invoke(mp,[||])
        let keys = 
            IEnumerableType.toList keysObj
            |> List.map(fun key -> key :?> int)
        let keys_e = mp.Keys |> Seq.toList
        Should.equal keys_e keys

    [<Fact>]
    member this.``get_Keys Test``() =
        let mp = Map [1,"1";2,"2"]
        let keys = IDictionaryType.get_Keys (mp.GetType()) mp
            
        let keys_e = mp.Keys |> Seq.toList |> List.map box
        Should.equal keys_e keys

    [<Fact>]
    member this.``ICollection of Map``() =
        let icty = typeof<ICollection<int>>
        for nf in icty.GetInterfaces() do
            output.WriteLine($"{nf.Name}")

        for mmb in icty.GetMembers() do
            output.WriteLine($"{mmb.Name}")

    [<Fact>]
    member this.``KeyValuePair of Map``() =
        let ty = typeof<Map<int,string>>
        let ity = ty.GetInterface("IEnumerable`1")
        let kvty = ity.GenericTypeArguments.[0]
        Should.equal kvty typeof<KeyValuePair<int,string>>
        let get_Key = KeyValuePairType.get_Key kvty
        let get_Value = KeyValuePairType.get_Value kvty

        let mp = Map [1,"1";2,"2"]
        let e = 
            let enm = IEnumerableType.getEnumerator mp
            [
                while enm.MoveNext() do
                    let kvp = enm.Current
                    get_Key kvp,get_Value kvp
            ]
        let y = mp |> Map.toList |> List.map(fun (k,v) -> box k,box v)
        Should.equal e y
        //for nf in ity.GetInterfaces() do
        //    output.WriteLine($"{nf.Name}")

        //for mmb in icty.GetMembers() do
        //    output.WriteLine($"{mmb.Name}")

    //[<Fact>]
    //member this.``makeArrayType``() =
    //    let x = typeof<Map<string,int>>
    //    let y = MapType.makeArrayType x
    //    Should.equal y typeof<(string*int)[]>

    //[<Fact>]
    //member this.``getToArray``() =
    //    let x = Map.ofList ["1", 1]
    //    let toArray = MapType.getToArray typeof<Map<string,int>>
    //    let y = toArray.Invoke(null,[|box x|]) :?> (string*int)[]

    //    Should.equal y [|"1", 1|]


    //[<Fact>]
    //member this.``getOfArray``() =
    //    let x = box [|"1", 1|]
    //    let ofArray = MapType.getOfArray typeof<Map<string,int>>
    //    let y = ofArray.Invoke(null,[|x|]) :?> Map<string,int>

    //    Should.equal y <| Map.ofList ["1", 1]

    //[<Fact>]
    //member this.``readMap``() =
    //    let x = Map.ofList ["1", 1]
    //    let read = MapType.readMap typeof<Map<string,int>>
    //    let ty,values = read x

    //    Should.equal ty typeof<string*int>
    //    Should.equal values [| box("1", 1)|]

    [<Fact>]
    member this.``Map type members``() =
        let ty = typeof<Map<int,string>>
        let elemType = ty.GenericTypeArguments.[0]
        Should.equal elemType typeof<int>
        for m in ty.GetMembers() do
            output.WriteLine($"{m}")
