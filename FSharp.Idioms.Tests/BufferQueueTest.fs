namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type BufferQueueTest (output: ITestOutputHelper) =

    [<Fact>]
    member this.``01 isEmpty test``() =
        let q = BufferQueue()

        Should.equal true q.isEmpty

        q.enqueue('0')
        Should.equal false q.isEmpty

        q.dequeueNothing()

        Should.equal false q.isEmpty

    [<Fact>]
    member this.``02 enqueue test``() =
        let xs = [0;1]
        let q = BufferQueue()
        for x in xs do
            q.enqueue(x)

        q.dequeueNothing()
        let ys =
            Seq.make q.tryNext
            |> Seq.toList
        Should.equal ys xs

    [<Fact>]
    member this.``03 dequeueToCurrent test``() =
        let xs = [0;1;2]
        let q = BufferQueue()
        for x in xs do
            q.enqueue(x)

        q.dequeueNothing()

        q.tryNext() |> ignore
        q.tryNext() |> ignore

        q.dequeueToCurrent()

        let y = q.tryNext().Value
        Should.equal y 2


    [<Fact>]
    member this.``04 enqueue error test``() =
        let q = BufferQueue()
        q.enqueue(0)
        q.enqueue(1)

        q.dequeueNothing()
        let ex = Assert.Throws<System.InvalidOperationException>(fun()->
            q.enqueue(2)
            )
        //output.WriteLine(ex.Message)
        Should.equal ex.Message "BufferQueue:Do not allow queue jumping."

    [<Fact>]
    member this.``05 dequeque empty test``() =
        let q = BufferQueue()

        q.dequeueNothing()
        q.dequeueToCurrent()
        let ex = Assert.Throws<System.InvalidOperationException>(fun()->
            q.dequeue(2)
            )
        //output.WriteLine(ex.Message)
        Should.equal ex.Message "BufferQueue:Not count enough queue."

    [<Fact>]
    member this.``06 dequeque right test``() =
        let q = BufferQueue()
        q.enqueue(1)
        q.enqueue(2)

        q.dequeue(2)
        Assert.True(q.isEmpty)

    [<Fact>]
    member this.``06 dequeque error test``() =
        let q = BufferQueue()
        q.enqueue('1')

        let ex = Assert.Throws<System.InvalidOperationException>(fun()->
            q.dequeue(2)
            )
        //output.WriteLine(ex.Message)
        Should.equal ex.Message "BufferQueue:Not count enough queue."




