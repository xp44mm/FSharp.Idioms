namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open System
open System.Text.RegularExpressions

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
            | On (tryMatch(Regex @"^\s+"))(m,r) -> (m,r)
            | _ -> failwith ""
        Should.equal y (" ", "abc")

    [<Fact>]
    member this.``usage or``() =
        let test (x & (LE 3 _ | GE 8 _ |Wild)) = x

        let x1 = test 2
        Should.equal 2 x1

        let x2 = test 9
        Should.equal 9 x2

        let x3 = Assert.Throws<ArgumentException>(fun()->
            test 5
            |> ignore
        )
        Should.equal "invalidArg" x3.Message

    [<Fact>]
    member this.``usage and``() =
        let test (x & (GT 3 _ & LT 8 _ |Wild)) = x

        let x1 = test 5
        Should.equal 5 x1

        let x3 = Assert.Throws<ArgumentException>(fun()->
            test 2
            |> ignore
        )

        Should.equal "invalidArg" x3.Message

        let x5 = Assert.Throws<ArgumentException>(fun()->
            test 9
            |> ignore
        )

        Should.equal "invalidArg" x5.Message


    [<Fact>]
    member this.``usage eq``() =
        let test (x & (EQ 3 _ | EQ 8 _ |Wild)) = x

        let x1 = test 3
        Should.equal 3 x1

        let x3 = Assert.Throws<ArgumentException>(fun()->
            test 2
            |> ignore
        )

        Should.equal "invalidArg" x3.Message

        let x5 = Assert.Throws<ArgumentException>(fun()->
            test 9
            |> ignore
        )

        Should.equal "invalidArg" x5.Message




