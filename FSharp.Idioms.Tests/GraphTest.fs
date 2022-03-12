namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

type GraphTest(output : ITestOutputHelper) =
    let show res =
        res
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``disjoint``() =
        let pairs = set [
            set [1;2]
            set [2;1]
            set [3;4]
            set [4;5]
            set [5;3]
        ]
        //合并有共同元素的集合元素
        let expected =
            set [
                set[1;2];
                set[3;4;5]
            ]
        let res = Graph.disjoint pairs
        Should.equal expected res

    [<Fact>]
    member this.``propagate cycle``() =
        /// 环
        let pairs = set [
             1,2
             2,1
        ]

        /// 自己包含自己
        let basis =
            pairs
            |> Seq.collect(fun(a,b)->[a;b])
            |> Seq.map(fun k -> k, Set.singleton k)
            |> Map.ofSeq

        let y = pairs |> Graph.propagate basis

        //show y

        let yy = Map.ofList [1,set [1;2];2,set [1;2]]

        Should.equal y yy

    [<Fact>]
    member this.``propagate single``() =
        /// 非环:左边是容器，右边是元素。
        let pairs = set [
             1,2
        ]

        /// 自己包含自己
        let basis = Map.ofList [
                //1, set [1]
                2, set [2]
            ]

        let y = pairs |> Graph.propagate basis

        //show y

        let yy = Map.ofList [1,set [2];2,set [2]]

        Should.equal y yy

    [<Fact>]
    member this.``propagate patterns string``() =
        let map = Map [
            ("NULL", set ["NULL"]); 
            ("anonPattern", set ["NULL"])]
        let remains = set [
            ("anonPattern", "anonPatterns"); 
            ("anonPatterns", "anonPattern");         
            ("pattern", "anonPattern")]

        let y = Graph.propagate map remains
        show y

    [<Fact>]
    member this.``TopologicalSort``() =
        let undershorts = "undershorts"
        let pants       = "pants"
        let belt        = "belt"
        let shirt       = "shirt"
        let tie         = "tie"
        let jacket      = "jacket"
        let socks       = "socks"
        let shoes       = "shoes"
        let watch       = "watch"

        let graph = 
            [
                socks,shoes
                undershorts,pants
                undershorts,shoes
                pants,belt
                pants,shoes
                shirt,belt
                shirt,tie
                belt,jacket
                tie,jacket

                //补充顺序
                //undershorts,watch
                //socks,shirt
                //socks,undershorts
                //undershorts,shirt          
                //pants,shirt
                //shoes,shirt
                //belt,tie
            ]

        let y = Graph.topologicalSort graph
        show y
