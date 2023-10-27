﻿namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Idioms.Literals
open FSharp.xUnit

type QueueTest(output: ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``empty``() =
        let q = 
            Queue.empty

        Assert.True(Queue.isEmpty q)

    [<Fact>]
    member this.``Iterator``() =
        let q = 
            Queue.empty
            |> Queue.enqueue 1
            |> Queue.enqueue 2
            |> Queue.enqueue 3

        let x, rest =
            q
            |> Queue.dequeue

        show q
        show x
        show rest
