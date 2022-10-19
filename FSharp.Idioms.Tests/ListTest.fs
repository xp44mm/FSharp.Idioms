namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit

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
    member this.``advanceWhile``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.advanceWhile (fun i -> i<3)

        let res = [2;1],[3;4;5]
        Should.equal y res

    [<Fact>]
    member this.``advance``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.advance 2

        let res = [2;1],[3;4;5]
        Should.equal y res

    [<Fact>]
    member this.``tap``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.tap(fun x -> output.WriteLine(sprintf "%A" x))

        Should.equal ls y


    [<Fact>]
    member this.``ofRevArray``() =
        let arr = [|5;4;3;2;1|]
        
        let y = 
            arr
            |> List.ofRevArray

        let ls = [1;2;3;4;5]

        Should.equal y ls

    [<Fact>]
    member this.``depthFirstSort 22-4``() =
        let nodes = 
            [ "u",["v";"x"];
              "v",["y"];
              "w",["y";"z"];
              "x",["v"];
              "y",["x"];
              "z",["z"];
            ] |> Map.ofList
        
        let y = List.depthFirstSort nodes "u"
        show y
        //注意：结果列表忽略无关的元素
        let e = ["u";"v";"y";"x"]
        Should.equal e y
        ()

    [<Fact>]
    member this.``depthFirstSort 22-5``() =
        let nodes = 
            [ 
            "u",["t";"v"];
            "v",["s";"w"];
            "w",["x";];
            "x",["z"];
            "y",["x"];
            "z",["y";"w"];
            "s",["z";"w"];
            "t",["u";"v"];
            ] 
            |> Map.ofList
        
        let y = List.depthFirstSort nodes "s"
        show y
        //注意：结果列表忽略无关的元素
        let e = ["s";"z";"y";"x";"w"]
        Should.equal e y
        ()


