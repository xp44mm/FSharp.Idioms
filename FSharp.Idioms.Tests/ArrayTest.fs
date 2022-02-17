﻿namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type ArrayTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``Pair Array toMap``() =
        // 有重复键
        let pairs = 
            [|
                1,1
                1,2
                1,2
                2,1
                2,1
                2,2
            |]

        let y = Array.toMap pairs
        //show y
        let res = 
            Map.ofList [
                1,[|1;2;2|];
                2,[|1;1;2|];
                ]
        Should.equal y res

    [<Fact>]
    member this.``Triple Array toJaggedMap``() =
        // 有重复键
        let triples = 
            [|
                (1,1,1)
                (1,2,2)
                (1,2,3)
                (2,1,4)
                (2,1,5)
                (2,2,6)
            |]
        let y = Array.toJaggedMap triples
        //show y
        let res = 
            Map.ofList [
                1,Map.ofList [
                    1,[|1|];
                    2,[|2;3|]
                    ];
                2,Map.ofList [
                    1,[|4;5|];
                    2,[|6|]
                    ];
                ]
        Should.equal y res

    [<Fact>]
    member this.``ofJaggedMap to Triple Array``() =
        let jaggedMap =
            Map.ofList [
                1,Map.ofList [
                    1,1;
                    2,2;
                    ];
                2,Map.ofList [
                    1,3;
                    2,4;
                    ];
                ]

        let y = Array.ofJaggedMap jaggedMap

        //show y
        let res = 
            [|
                (1,1,1)
                (1,2,2)
                (2,1,3)
                (2,2,4)
            |]

        Should.equal y res

    [<Fact>]
    member this.``tap``() =
        let x = [|1;2;3;4;5|]
        
        let array = Array.create 5 0
        let y = 
            x
            |> Array.tapi(fun i x -> array.[i]<-x)

        Should.equal array x


    [<Fact>]
    member this.``ofRevArray``() =
        let arr = [|5;4;3;2;1|]
        
        let y = 
            arr
            |> Array.toRevList

        let ls = [1;2;3;4;5]

        Should.equal y ls

