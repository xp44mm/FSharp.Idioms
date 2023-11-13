namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System.Reflection

type ArrayTypeTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``Array Type Test``() =
        let ty = typeof<System.Array>
        for m in ty.GetMembers(BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.DeclaredOnly) do
            output.WriteLine($"{m}")

    [<Fact>]
    member this.``Array get_Length Test``() =
        let ty = typeof<System.Array>
        let get_Length = ty.GetMethod("get_Length")
        output.WriteLine($"{get_Length}")

    [<Fact>]
    member this.``Array Length Test``() =
        let ty = typeof<System.Array>
        let Length = ty.GetProperty("Length")
        output.WriteLine($"{Length}")

    [<Fact>]
    member this.``Array GetValue Test``() =
        let ty = typeof<System.Array>
        let GetValue = ty.GetMethod("GetValue",[|typeof<int>|])
        output.WriteLine($"{GetValue}")

    [<Fact>]
    member this.``getLength``() =
        let x = [|1;2|]
        let y = ArrayType.length x
        Should.equal y 2

    [<Fact>]
    member this.``invokeGetValue``() =
        let x = [|1;2|]
        let y = ArrayType.getValue 1 x
        Should.equal y 2

    [<Fact>]
    member this.``toArray empty``() =
        let x:int[] = [||]
        let values = 
            ArrayType.toArray x
            |> Array.map unbox<int>
        Assert.Equal<int>(values, x)

    [<Fact>]
    member this.``toArray``() =
        let x = [|1;2|]
        let values = 
            ArrayType.toArray x
            |> Array.map unbox<int>
        Assert.Equal<int>(values, x)
