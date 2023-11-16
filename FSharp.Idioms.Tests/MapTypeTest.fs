namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Idioms
open FSharp.xUnit
open System.Reflection
open System.Collections.Generic

type MapTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

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

    //[<Fact>]
    //member this.``getElementType``() =
    //    let x = typeof<Map<string,int>> 
    //    let y = MapType.getElementType x
    //    Should.equal y typeof<string*int>


    //[<Fact>]
    //member this.``makeArrayType``() =
    //    let x = typeof<Map<string,int>>
    //    let y = MapType.makeArrayType x
    //    Should.equal y typeof<(string*int)[]>

    [<Fact>]
    member this.``getToArray``() =
        let x = Map.ofList ["1", 1]
        let toArray = MapType.getToArray typeof<Map<string,int>>
        let y = toArray.Invoke(null,[|box x|]) :?> (string*int)[]

        Should.equal y [|"1", 1|]


    [<Fact>]
    member this.``getOfArray``() =
        let x = box [|"1", 1|]
        let ofArray = MapType.getOfArray typeof<Map<string,int>>
        let y = ofArray.Invoke(null,[|x|]) :?> Map<string,int>

        Should.equal y <| Map.ofList ["1", 1]

    //[<Fact>]
    //member this.``readMap``() =
    //    let x = Map.ofList ["1", 1]
    //    let read = MapType.readMap typeof<Map<string,int>>
    //    let ty,values = read x

    //    Should.equal ty typeof<string*int>
    //    Should.equal values [| box("1", 1)|]
