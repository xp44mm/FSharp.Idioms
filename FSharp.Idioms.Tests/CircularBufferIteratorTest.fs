namespace FSharp.Idioms

open Xunit

open FSharp.xUnit

type CircularBufferIteratorTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``tryNext test``() =
        let xs = [ 'a'; 'b' ]
        let iterator = CircularBufferIterator(xs)
        let ys =
            [
                iterator.tryNext()
                iterator.tryNext()
                iterator.tryNext()
                iterator.tryNext()
            ]
        let es = [ Some 'a'; Some 'b'; None; None ]
        Should.equal es ys

    [<Fact>]
    member this.``ongoing test``() =
        let xs = [ 'a'; 'b' ]
        let iterator = CircularBufferIterator(xs)

        // 缓存迭代器用法
        let rec loop () =
            seq {
                match iterator.tryNext() with
                | Some elem ->
                    iterator.dequeue(1)
                    yield elem
                    yield! loop()
                | None -> ()

            }
        let ys = loop() |> Seq.toList

        Should.equal xs ys

    [<Fact>]
    member this.``reset Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = CircularBufferIterator(iterable)
        let y =
            [
                iterator.tryNext().Value
                iterator.tryNext().Value
                do iterator.reset()
                iterator.tryNext().Value
                iterator.tryNext().Value
                iterator.tryNext().Value //测试衔接是否正确
            ]
        let e = [ 1; 2; 1; 2; 3 ]
        Should.equal e y

    [<Fact>]
    member this.``dequeueHead Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = CircularBufferIterator(iterable)
        let y =
            [
                iterator.tryNext().Value
                iterator.tryNext().Value
                do iterator.dequeue(1)
                iterator.tryNext().Value
                iterator.tryNext().Value
                iterator.tryNext().Value
            ]
        let e = [ 1; 2; 2; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``dequeueToCurrent Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = CircularBufferIterator(iterable)
        let y =
            [
                iterator.tryNext().Value
                iterator.tryNext().Value
                do iterator.dequeue(2)
                iterator.tryNext().Value
                iterator.tryNext().Value
                iterator.tryNext().Value
            ]
        let e = [ 1; 2; 3; 4; 5 ]
        Should.equal y e

    [<Fact>]
    member this.``dequeue count Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = CircularBufferIterator(iterable)
        let y =
            [
                iterator.tryNext().Value
                iterator.tryNext().Value
                iterator.tryNext().Value
                do iterator.dequeue(2)
                iterator.tryNext().Value
                iterator.tryNext().Value
            ]
        let e = [ 1; 2; 3; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``GetEnumerator Current``() =
        let sq = Seq.empty
        let e = sq.GetEnumerator()

        let ex =
            Assert.Throws<System.InvalidOperationException>(fun _ ->
                e.Current |> ignore
            )
        output.WriteLine(ex.Message)
        e.MoveNext() |> ignore
        e.MoveNext() |> ignore
        let ex =
            Assert.Throws<System.InvalidOperationException>(fun _ ->
                e.Current |> ignore
            )
        output.WriteLine(ex.Message)
