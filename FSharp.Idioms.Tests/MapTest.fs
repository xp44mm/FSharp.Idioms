namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.xUnit
open System.Collections.Generic

type MapTest(output : ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``fromDuplicateKeys test``() =
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
    member this.``mapKey test``() =
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
    member this.``mapEntry test``() =
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
    member this.``concat test``() =
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
    member this.``append test``() =
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
    member this.``keys test``() =
        let mp1 = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList

        let y = Map.keys mp1
        //show y
        let res = set ["1";"2"]
        Should.equal y res

    [<Fact>]
    member this.``from interface``() =
        let dict = new Dictionary<string,int>()
        [
            "1"    , 1
            "2"    , 2
        ]
        |> List.iter(fun(k,v)->
            dict.Add(k,v)
        )

        let y = Map.fromInterface dict
        let mp = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList

        Should.equal y mp

    [<Fact>]
    member this.``key is subset or superset or equal``() =
        let mp1 = 
            [
                "1"    , 1
            ] |> Map.ofList

        let mp2 = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList

        let mp22 = 
            [
                "1"    , 1
                "2"    , 2
            ] |> Map.ofList


        Assert.True(Map.keyIsSubset mp1 mp2)
        Assert.True(Map.keyIsSuperset mp2 mp1)
        Assert.True(Map.keyIsEqual mp2 mp22)

    [<Fact>]
    member this.``intersectByKey``() =
        let mp1 = 
            [
                "0"    , 0
                "1"    , 1

            ] |> Map.ofList

        let mp2 = 
            [
                "1"    , 11
                "2"    , 22
            ] |> Map.ofList

        let y = Map.intersectByKey mp1 mp2

        Should.equal y ["1",(1,11)]
    [<Fact>]
    member this.``differenceByKey``() =
        let mp1 = 
            [
                "0"    , 0
                "1"    , 1

            ] |> Map.ofList

        let mp2 = 
            [
                "1"    , 1.1
                "2"    , 2.2
            ] |> Map.ofList

        let y = Map.differenceByKey mp1 mp2
        Should.equal y (Map.ofList ["0",0])

    [<Fact>]
    member this.``inverse test``() =
        let mp = 
            Map [
                "0"    , 0
                "1"    , 1
            ]
        let y = mp |> Map.inverse
        let e = Map [0,["0"];1,["1"]]
        show y
        Should.equal e y
