namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

//open FSharp.Idioms.ActivePatterns

type ActivePatternsTest(output : ITestOutputHelper) =
    let show res =
        res
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``on``() =
        let x = " abc"
        let y =        
            match x with
            | On (tryPrefix @"\s+")(m,r) -> (m,r)
            | _ -> failwith ""
        Should.equal y (" ", "abc")