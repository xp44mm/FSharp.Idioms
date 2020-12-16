namespace FSharp.Idioms.Tests

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open FSharp.Idioms

type ListTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``test penetrate``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.penetrate (fun i -> i<=3) 

        let res = [1;2;3;4]
        Should.equal y res

