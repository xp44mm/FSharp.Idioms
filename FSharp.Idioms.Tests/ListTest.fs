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
    member this.``Pair List toMap``() =
        // 有重复键
        let pairs = [
            1,1
            1,2
            1,2
            2,1
            2,1
            2,2
        ]

        let y = List.toMap pairs
        //show y
        let res = 
            Map.ofList [
                1,[1;2;2];
                2,[1;1;2];
                ]
        Should.equal y res

    [<Fact>]
    member this.``Triple List toJaggedMap``() =
        // 有重复键
        let triples = 
            [
                (1,1,1)
                (1,2,2)
                (1,2,3)
                (2,1,4)
                (2,1,5)
                (2,2,6)
            ]
        let y = List.toJaggedMap triples
        //show y
        let res = 
            Map.ofList [
                1,Map.ofList [
                    1,[1];
                    2,[2;3]
                    ];
                2,Map.ofList [
                    1,[4;5];
                    2,[6]
                    ];
                ]
        Should.equal y res

    [<Fact>]
    member this.``ofJaggedMap to Triple List``() =
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

        let y = List.ofJaggedMap jaggedMap

        //show y
        let res = 
            [
                (1,1,1)
                (1,2,2)
                (2,1,3)
                (2,2,4)
            ]

        Should.equal y res


    [<Fact>]
    member this.``test takeUntilNot``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.takeUntilNot (fun i -> i<=3)

        let res = [1;2;3;4]
        Should.equal y res

