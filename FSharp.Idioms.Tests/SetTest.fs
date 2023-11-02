namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System.Collections.Generic

type SetTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Literal.stringify
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
        //Should.equal y res
        Assert.Equal<HashSet<_>>(y,res)

    [<Fact>]
    member this.``unionByKey``() =
        let mp = 
            [
                1,set [1;2]; 
                1,set [3;4];
                2,set [5;6]
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

    [<Fact>]
    member this.``combine2``() =
        let st = set [0..4]
        let y = st |> Set.combine2
        show y

        let res = set [
            0,1;0,2;0,3;0,4;
            1,2;1,3;1,4;
            2,3;2,4;
            3,4]

        Should.equal y res

    [<Fact>]
    member this.``allPairs``() =
        let st = set [0..4]
        let y = (st,st) ||> Set.allPairs
        show y

        let res = set [
            0,0;0,1;0,2;0,3;0,4;
            1,0;1,1;1,2;1,3;1,4;
            2,0;2,1;2,2;2,3;2,4;
            3,0;3,1;3,2;3,3;3,4;
            4,0;4,1;4,2;4,3;4,4]

        Should.equal y res

    [<Theory>]
    [<InlineData(2,2)>]
    [<InlineData(5,-1)>]
    member this.``getIndex``(elem,exp) =
        let st = set [0..4]
        let y = Set.getIndex elem st
        Should.equal y exp

