namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open System
open System.Text.RegularExpressions
open FSharp.Idioms.ActivePatterns

type ActivePatternsTest(output : ITestOutputHelper) =
    let show res =
        res
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``on``() =
        let x = [3;5]
        let y =        
            match x with
            | On (List.tryFindIndex((=) 5)) i -> i
            | _ -> failwith ""
        Should.equal y 1

    [<Fact>]
    member this.``Wild``() =
        let x = [3;4]

        let er = Assert.Throws<exn>(fun()->
            match x with
            | On (List.tryFindIndex((=) 5)) i -> i
            | [] | Wild -> 0
            |> ignore
        )
        Should.equal er.Message "Wild:[3; 4]"

