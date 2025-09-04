namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

open FSharp.Idioms.Eulerian
open System

type EulerianTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``Vector rev test``() =
        let v = Vector.just ((0, 0), (5, 10), true)
        let reversed = v.rev ()

        Assert.Equal((5, 10), reversed.startPoint)
        Assert.Equal((0, 0), reversed.endPoint)
        Assert.True(reversed.penUp)

    [<Fact>]
    member this.``Line create test - valid input``() =
        let points = set [ (0, 0); (5, 10) ]
        let line = Line.create (points, false)

        Should.equal points line.points
        Assert.False(line.penUp)

    [<Fact>]
    member this.``Line create test - invalid input``() =
        let points =
            set [
                (0, 0)
                (5, 10)
                (15, 20)
            ]

        Assert.Throws<Exception>(fun () -> Line.create (points, false) |> ignore)

    [<Fact>]
    member this.``Line toVectors test``() =
        let points = set [ (0, 0); (5, 10) ]
        let line = Line.create (points, true)

        let vectors = line.toVectors ()

        Assert.Equal(2, vectors.Length)

        let v1 = vectors.[0]
        Assert.Equal((0, 0), v1.startPoint)
        Assert.Equal((5, 10), v1.endPoint)
        Assert.True(v1.penUp)

        let v2 = vectors.[1]
        Assert.Equal((5, 10), v2.startPoint)
        Assert.Equal((0, 0), v2.endPoint)
        Assert.True(v2.penUp)

    [<Fact>]
    member this.``getLineSet test - no duplicates``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (5, 10); (15, 20) ], false)
        ]

        let lineSet = getLineSet lines
        Assert.Equal(2, lineSet.Count)

    [<Fact>]
    member this.``getLineSet test - with duplicates``() =
        let line = Line.create (set [ (0, 0); (5, 10) ], true)
        let lines = [ line; line ]

        Assert.Throws<Exception>(fun () -> getLineSet lines |> ignore)

    [<Fact>]
    member this.``forwardNextVector test - found``() =
        let l1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let l2 = Line.create (set [ (5, 10); (15, 20) ], false)

        let vectors = set [ yield! l1.toVectors (); yield! l2.toVectors () ]

        let result = forwardNextVector vectors (0, 0)
        Assert.True(result.IsSome)

        let (vec, rest) = result.Value
        Assert.Equal((0, 0), vec.startPoint)
        Assert.Equal((5, 10), vec.endPoint)
        Assert.Equal(2, rest.Count)

    [<Fact>]
    member this.``forwardNextVector test - not found``() =
        let l1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let l2 = Line.create (set [ (5, 10); (15, 20) ], false)

        let vectors = set [ yield! l1.toVectors (); yield! l2.toVectors () ]

        let result = forwardNextVector vectors (10, 10)
        Assert.True(result.IsNone)

    [<Fact>]
    member this.``lastPoint test``() =
        let revSortedVectors = [
            Vector.just ((0, 0), (5, 10), true)
            Vector.just ((5, 10), (15, 20), false)
        ]

        let point = lastPoint revSortedVectors
        Assert.Equal((5, 10), point)

    [<Fact>]
    member this.``firstPoint test``() =
        let sortedVectors = [
            Vector.just ((0, 0), (5, 10), true)
            Vector.just ((5, 10), (15, 20), false)
        ]

        let point = firstPoint sortedVectors
        Assert.Equal((0, 0), point)

    [<Fact>]
    member this.``basicSort test - simple path``() =
        let l1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let l2 = Line.create (set [ (5, 10); (15, 20) ], false)

        let vectors = set [ yield! l1.toVectors (); yield! l2.toVectors () ]

        let p0 = (0, 0)
        match forwardNextVector vectors p0 with
        | None -> ()
        | Some(startVector, restVectors) ->
            let sorted, rest = basicSort [ startVector ] restVectors

            Assert.Equal(2, sorted.Length)
            Assert.Equal(0, rest.Count)

            Assert.Equal((0, 0), sorted.[0].startPoint)
            Assert.Equal((5, 10), sorted.[0].endPoint)

            Assert.Equal((5, 10), sorted.[1].startPoint)
            Assert.Equal((15, 20), sorted.[1].endPoint)

    [<Fact>]
    member this.``basicSort test - with cycle``() =
        let p0 = (0, 0)

        let l1 = Line.create (set [ p0; (5, 10) ], true)
        let l2 = Line.create (set [ (5, 10); (15, 20) ], false)
        let l3 = Line.create (set [ (15, 20); p0 ], false)

        let vectors =
            set [
                yield! l1.toVectors ()
                yield! l2.toVectors ()
                yield! l3.toVectors ()
            ]

        match forwardNextVector vectors p0 with
        | None -> ()
        | Some(startVector, restVectors) ->

            let sorted, rest = basicSort [ startVector ] restVectors
            Should.equal sorted [
                Vector.just ((0, 0), (5, 10), true)
                Vector.just ((5, 10), (15, 20), false)
                Vector.just ((15, 20), p0, false)
            ]
            Assert.True(rest.IsEmpty)

    [<Fact>]
    member this.``Integration test with provided lines``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (5, 10); (15, 20) ], false)
            Line.create (set [ (15, 20); (30, -40) ], false)
        ]

        // 将所有线的向量合并
        let allVectors = lines |> List.collect (fun line -> line.toVectors ()) |> Set.ofList

        // 从第一个点开始
        let startPoint = (0, 0)
        let startVectorOption = forwardNextVector allVectors startPoint

        Assert.True(startVectorOption.IsSome)

        let (startVector, initialRest) = startVectorOption.Value

        let sorted, rest = basicSort [ startVector ] initialRest

        // 检查排序后的路径
        output.WriteLine($"Sorted vectors count: {sorted.Length}")
        output.WriteLine($"Rest vectors count: {rest.Count}")

        // 应该能找到完整路径
        Assert.Equal(3, sorted.Length)
        Assert.Equal(0, rest.Count)

        // 检查路径连续性
        Assert.Equal((0, 0), sorted.[0].startPoint)
        Assert.Equal((5, 10), sorted.[0].endPoint)

        Assert.Equal((5, 10), sorted.[1].startPoint)
        Assert.Equal((15, 20), sorted.[1].endPoint)

        Assert.Equal((15, 20), sorted.[2].startPoint)
        Assert.Equal((30, -40), sorted.[2].endPoint)

    [<Fact>]
    member this.``insertRing test - no rings``() =
        // 简单路径，没有环
        let basePath = [
            Vector.just ((0, 0), (5, 10), true)
            Vector.just ((5, 10), (15, 20), false)
        ]
        let restVectors = Set.empty

        let result = insertRing [] basePath restVectors

        Assert.Equal(2, result.Length)
        Assert.Equal((0, 0), result.[0].startPoint)
        Assert.Equal((5, 10), result.[0].endPoint)
        Assert.Equal((5, 10), result.[1].startPoint)
        Assert.Equal((15, 20), result.[1].endPoint)

    [<Fact>]
    member this.``insertRing test - with ring at start point``() =
        // 主路径: (0,0) -> (5,10) -> (15,20)
        // 环: (0,0) -> (2,3) -> (0,0)
        let basePath = [
            Vector.just ((0, 0), (5, 10), true)
            Vector.just ((5, 10), (15, 20), false)
        ]
        let restVectors =
            set [ Vector.just ((0, 0), (2, 3), true); Vector.just ((2, 3), (0, 0), false) ]

        let result = insertRing [] basePath restVectors

        // 应该为: (0,0)->(2,3), (2,3)->(0,0), (0,0)->(5,10), (5,10)->(15,20)
        Assert.Equal(4, result.Length)
        Assert.Equal((0, 0), result.[0].startPoint)
        Assert.Equal((2, 3), result.[0].endPoint)
        Assert.Equal((2, 3), result.[1].startPoint)
        Assert.Equal((0, 0), result.[1].endPoint)
        Assert.Equal((0, 0), result.[2].startPoint)
        Assert.Equal((5, 10), result.[2].endPoint)
        Assert.Equal((5, 10), result.[3].startPoint)
        Assert.Equal((15, 20), result.[3].endPoint)

    [<Fact>]
    member this.``sortVectors test - simple path``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (5, 10); (15, 20) ], false)
        ]

        let result = sortVectors lines

        Assert.Equal(2, result.Length)
        Assert.Equal((0, 0), result.[0].startPoint)
        Assert.Equal((5, 10), result.[0].endPoint)
        Assert.Equal((5, 10), result.[1].startPoint)
        Assert.Equal((15, 20), result.[1].endPoint)

    [<Fact>]
    member this.``sortVectors test - with rings``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (5, 10); (15, 20) ], false)
            Line.create (set [ (0, 0); (2, 3) ], true) // 环的一部分
            Line.create (set [ (2, 3); (0, 0) ], false) // 环的另一部分，penUp相同但点不同
        ]

        let result = sortVectors lines

        output.WriteLine($"Result length: {result.Length}")
        for i, vec in result |> List.indexed do
            output.WriteLine($"{i}: {vec.startPoint} -> {vec.endPoint} (penUp: {vec.penUp})")

        // 应该包含环
        Assert.True(result.Length >= 4)

    [<Fact>]
    member this.``sortVectors test - provided example``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (5, 10); (15, 20) ], false)
            Line.create (set [ (15, 20); (30, -40) ], true)
        ]

        let result = sortVectors lines

        Assert.Equal(3, result.Length)
        Assert.Equal((0, 0), result.[0].startPoint)
        Assert.Equal((5, 10), result.[0].endPoint)
        Assert.True(result.[0].penUp)

        Assert.Equal((5, 10), result.[1].startPoint)
        Assert.Equal((15, 20), result.[1].endPoint)
        Assert.False(result.[1].penUp)

        Assert.Equal((15, 20), result.[2].startPoint)
        Assert.Equal((30, -40), result.[2].endPoint)
        Assert.True(result.[2].penUp)

    [<Fact>]
    member this.``sortVectors test - should start from origin``() =
        let lines = [
            Line.create (set [ (1, 1); (2, 2) ], true) // 不从原点开始
        ]

        Assert.Throws<System.Exception>(fun () -> sortVectors lines |> ignore)

    [<Fact>]
    member this.``sortVectors test - disconnected lines should fail``() =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (20, 20); (25, 25) ], true) // 断开连接
        ]

        Assert.Throws<System.Exception>(fun () -> sortVectors lines |> ignore)

    [<Fact>]
    member this.``sortVectors test - lines with same points but different penUp should both be kept``
        ()
        =
        let lines = [
            Line.create (set [ (0, 0); (5, 10) ], true)
            Line.create (set [ (0, 0); (5, 10) ], false) // 相同的点，不同的 penUp
        ]

        // 这两条线应该都被保留，因为 penUp 不同
        let result = sortVectors lines

        Assert.Equal(2, result.Length)
        Assert.True(result.[0].penUp <> result.[1].penUp)

    [<Fact>]
    member this.``getLineSet test - keeps lines with different penUp``() =
        let line1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let line2 = Line.create (set [ (0, 0); (5, 10) ], false) // 相同的点，不同的 penUp

        let lines = [ line1; line2 ]
        let lineSet = getLineSet lines

        Assert.Equal(2, lineSet.Count)

    [<Fact>]
    member this.``getLineSet test - throws on duplicate lines with same penUp``() =
        let line = Line.create (set [ (0, 0); (5, 10) ], true)
        let lines = [ line; line ] // 完全相同的线

        // getLineSet 应该会抛出异常
        Assert.Throws<System.Exception>(fun () -> getLineSet lines |> ignore)

    [<Fact>]
    member this.``getLineSet test - throws on duplicate lines``() =
        let line = Line.create (set [ (0, 0); (5, 10) ], true)
        let lines = [ line; line ] // 重复的线

        // getLineSet 应该抛出异常
        Assert.Throws<System.Exception>(fun () -> getLineSet lines |> ignore)

    [<Fact>]
    member this.``getLineSet test - removes duplicate lines when using distinct first``() =
        let line = Line.create (set [ (0, 0); (5, 10) ], true)
        let lines = [ line; line ] // 重复的线

        let distinctLines = lines |> List.distinct
        let lineSet = getLineSet distinctLines
        Assert.Single(lineSet)

    [<Fact>]
    member this.``sortVectors test - should throw on duplicate lines``() =
        let line1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let line2 = Line.create (set [ (0, 0); (5, 10) ], true) // 完全相同的线

        let lines = [ line1; line2 ]

        // sortVectors 应该抛出异常，因为 getLineSet 会检测到重复
        Assert.Throws<System.Exception>(fun () -> sortVectors lines |> ignore)

    [<Fact>]
    member this.``sortVectors test - works with distinct lines``() =
        let line1 = Line.create (set [ (0, 0); (5, 10) ], true)
        let line2 = Line.create (set [ (0, 0); (5, 10) ], true) // 完全相同的线

        // 手动去重后再测试
        let distinctLines = [ line1 ] // 只保留一个
        let result = sortVectors distinctLines

        Assert.Single(result) // 应该只有一条线
