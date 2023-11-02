namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type ArrayTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``getElementType``() =
        let x = typeof<int[]> 
        let y = ArrayType.getElementType x
        Should.equal y typeof<int>

    [<Fact>]
    member this.``getLength``() =
        let x = [|1;2|]
        let len = ArrayType.getLength <| x.GetType()
        let y = len x
        Should.equal y 2

    [<Fact>]
    member this.``invokeGetValue``() =
        let x = [|1;2|]
        let getValue = ArrayType.invokeGetValue <| x.GetType()
        let y = getValue(x,1) :?> int
        Should.equal y 2

    [<Fact>]
    member this.``readArray empty``() =
        let x:int[] = [||]
        let read = ArrayType.readArray <| x.GetType()
        let tp, values = read x
        Should.equal tp typeof<int>
        Should.equal values (Array.map box x)

    [<Fact>]
    member this.``readArray``() =
        let x = [|1;2|]
        let read = ArrayType.readArray <| x.GetType()
        let tp, values = read x
        Should.equal tp typeof<int>
        Should.equal values (Array.map box x)
