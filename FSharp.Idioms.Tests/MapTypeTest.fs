namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Idioms
open FSharp.xUnit

type MapTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getElementType``() =
        let x = typeof<Map<string,int>> 
        let y = MapType.getElementType x
        Should.equal y typeof<string*int>

    [<Fact>]
    member this.``makeArrayType``() =
        let x = typeof<Map<string,int>>
        let y = MapType.makeArrayType x
        Should.equal y typeof<(string*int)[]>

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

    [<Fact>]
    member this.``readMap``() =
        let x = Map.ofList ["1", 1]
        let read = MapType.readMap typeof<Map<string,int>>
        let ty,values = read x

        Should.equal ty typeof<string*int>
        Should.equal values [| box("1", 1)|]
