namespace FSharp.Idioms.Tests

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open FSharp.Idioms

type MapTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``test fromDuplicateKeys``() =
        let pairs = 
            [
                1,"1"
                1,"1.0"
                2,"2"
                2,"2.0"
                3,"3"
            ]
        let y = 
            Map.fromDuplicateKeys pairs 
            |> Map.map(fun _ sq -> List.ofSeq sq)
        //show y
        let res = Map.ofList [1,["1";"1.0"];2,["2";"2.0"];3,["3"]]
        Should.equal y res

    [<Fact>]
    member this.``test mapKey``() =
        let mp = 
            [
                "1"    , 1
                "1.0"  , 1
                "2"    , 2
                "2.0"  , 2
                "3"    , 3
            ] |> Map.ofList
        let getKey k v = System.Double.Parse(k)
        let y = mp |> Map.mapKey getKey |> Map.map(fun _ sq -> List.ofSeq sq)
        let res = Map.ofList [1.0,[1;1];2.0,[2;2];3.0,[3]]
        //show y
        Should.equal y res

    [<Fact>]
    member this.``test mapEntry``() =
        let mp = Map.ofList [
                "1"    , 1
                "1.0"  , 1
                "2"    , 2
                "2.0"  , 2
                "3"    , 3
            ]
        let getEntry k v = 
            let k = System.Double.Parse(k)
            k,v
        let y = mp |> Map.mapEntry getEntry |> Map.map(fun _ sq -> List.ofSeq sq)
        let res = Map.ofList [1.0,[1;1];2.0,[2;2];3.0,[3]]
        //show y
        Should.equal y res

    [<Fact>]
    member this.``test concat``() =
        let mp1 = 
            Map.ofList [
                "1"    , 1
                "2"    , 2
            ]
        let mp2 = 
            Map.ofList [
                "2"    , 2
                "3"    , 3
            ]
        let mp3 = 
            Map.ofList [
                "3"    , 3
                "4"    , 4
            ]

        let y = Map.concat [mp1;mp2;mp3] |> Map.map(fun _ sq -> List.ofSeq sq)

        //show y
        let res = Map.ofList ["1",[1];"2",[2;2];"3",[3;3];"4",[4]]
        Should.equal y res

    [<Fact>]
    member this.``test append``() =
        let mp1 = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList

        let mp2 = 
            [
                "2"    , 2
                "3"    , 3
            ] |> Map.ofList

        let y = Map.append mp1 mp2 |> Map.map(fun _ sq -> List.ofSeq sq)

        //show y
        let res = Map.ofList ["1",[1];"2",[2;2];"3",[3]]
        Should.equal y res

    [<Fact>]
    member this.``test keys``() =
        let mp1 = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList

        let y = Map.keys mp1
        //show y
        let res = set ["1";"2"]
        Should.equal y res


