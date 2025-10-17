namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type BufferIteratorTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``tryNext test``() =
        let xs = [ 'a'; 'b' ]
        let iterator = BufferIterator(xs)
        let ys = [
            iterator.tryNext ()
            iterator.tryNext ()
            iterator.tryNext ()
            iterator.tryNext ()
        ]
        let es = [ Some 'a'; Some 'b'; None; None ]
        Should.equal es ys

    [<Fact>]
    member this.``ongoing test``() =
        let xs = [ 'a'; 'b' ]
        let iterator = BufferIterator(xs)

        // 缓存迭代器用法
        let ys = [

            while iterator.ongoing() do
                match iterator.tryNext () with
                | Some elem ->
                    iterator.dequeue (1)
                    elem
                | None -> ()
        ]
        Should.equal xs ys

    [<Fact>]
    member this.``dequequeNothing Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = BufferIterator(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.dequeue (0)
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value //测试衔接是否正确
        ]
        let e = [ 1; 2; 1; 2; 3 ]
        Should.equal y e

    [<Fact>]
    member this.``dequeueHead Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = BufferIterator(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.dequeueHead ()
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 2; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``dequeueToCurrent Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = BufferIterator(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.dequeueToCurrent ()
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 3; 4; 5 ]
        Should.equal y e

    [<Fact>]
    member this.``dequeue count Iterator``() =
        let iterable = Seq.ofList [ 1..5 ]
        let iterator = BufferIterator(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.dequeue (2)
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 3; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``GetEnumerator Current``() =
        let sq = Seq.empty
        let e = sq.GetEnumerator()

        let ex = Assert.Throws<System.InvalidOperationException>(
            fun _ ->
                e.Current
                |> ignore
        )
        output.WriteLine(ex.Message)
        e.MoveNext() |> ignore
        e.MoveNext() |> ignore
        let ex = Assert.Throws<System.InvalidOperationException>(
            fun _ ->
                e.Current
                |> ignore
        )
        output.WriteLine(ex.Message)
