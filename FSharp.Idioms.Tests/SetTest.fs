namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type SetTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine


    [<Fact>]
    member this.``ofHashSet``() =
        let hset = HashSet([|1;2;3|])
        let y = Set.ofHashSet hset
        let res = Set.ofList [1;2;3]
        Should.equal y res

    [<Fact>]
    member this.``toHashSet``() =
        let set = Set.ofList [1;2;3]
        let y = Set.toHashSet set
        let res = HashSet([|1;2;3|])
        Should.equal y res

    [<Fact>]
    member this.``unionByKey``() =
        let mp = 
            [
                1,set[1;2]; 
                1,set[3;4];
                2,set[5;6]
            ]

        let expected = set [
            (1,Set.ofArray [|1;2;3;4|]);
            (2,Set.ofArray [|5;6|])]
        let res = Set.unionByKey mp
        Should.equal expected res

    [<Fact>]
    member this.``Pair Set toMap``() =
        // 输入特点，键有重复，但pair本身不重复。
        let pairs = 
            set [
                1,1
                1,2
                1,3
                2,1
                2,2
                2,3
            ]

        let y = Set.toMap pairs

        //show y
        let res = 
            Map.ofList [
                1,set [1;2;3];
                2,set [1;2;3];
                ]
        Should.equal y res

    [<Fact>]
    member this.``ofJaggedMap to Triple Set``() =
        let mp = 
            Map.ofList [
                1,Map.ofList [
                    1,1;
                    2,2;
                    ];
                2,Map.ofList [
                    1,3
                    2,4
                    ];
                ]

        let y = Set.ofJaggedMap mp

        let res = set [
            1,1,1;
            1,2,2;
            2,1,3;
            2,2,4]

        //show y
        Should.equal y res

    [<Fact>]
    member this.``Triple Set toJaggedMap``() =
        // 有重复键
        let triples = 
            set [
                (1,1,1)
                (1,2,2)
                (1,2,3)
                (2,1,4)
                (2,1,5)
                (2,2,6)
            ]
        let y = Set.toJaggedMap triples
        //show y
        let res = 
            Map.ofList [
                1,Map.ofList [
                    1,set [1];
                    2,set [2;3]];
                2,Map.ofList [
                    1,set [4;5];
                    2,set [6]]
                ]
        Should.equal y res

    [<Fact>]
    member this.``groupBy``() =
        let st = set [0..9]
        let y = st |> Set.groupBy (fun i -> i % 3 )
        //show y
        let res = set [0,set [0;3;6;9];1,set [1;4;7];2,set [2;5;8]]
        Should.equal y res
