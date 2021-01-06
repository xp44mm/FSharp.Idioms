namespace FSharp.Idioms.Tests

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open FSharp.Idioms

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
