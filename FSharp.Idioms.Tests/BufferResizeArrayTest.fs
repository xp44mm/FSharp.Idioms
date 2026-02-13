namespace FSharp.Idioms

open Xunit

open FSharp.xUnit

type BufferResizeArrayTest(output: ITestOutputHelper) =
    do ()

//[<Fact>]
//member this.``01 count test``() =
//    let q = BufferResizeArray()
//    Should.equal q.count 0

//    q.enqueue('a')
//    Should.equal q.count 1

//    q.dequeue(0)
//    Should.equal q.count 1

//[<Fact>]
//member this.``02 enqueue test``() =
//    let xs = ['0';'1']
//    let q = BufferResizeArray()
//    for x in xs do
//        q.enqueue(x)

//    q.dequeue(0)
//    let ys =
//        Seq.make q.tryNext
//        |> Seq.toList
//    Should.equal xs ys

//[<Fact>]
//member this.``03 dequeueToCurrent test``() =
//    let xs = ['0';'1';'2']
//    let q = BufferResizeArray()
//    for x in xs do
//        q.enqueue(x)

//    q.dequeue(0)

//    q.tryNext() |> ignore
//    q.tryNext() |> ignore

//    Should.equal q.current 1

//    q.dequeue(q.current + 1) // dequeueToCurrent

//    let y = q.tryNext().Value
//    Should.equal y '2'

//[<Fact>]
//member this.``04 enqueue error test``() =
//    let q = BufferResizeArray()
//    q.enqueue('0')
//    q.enqueue('1')
//    q.dequeue(0)
//    // 指针不在缓存末尾时候不可以入队操作
//    let ex = Assert.Throws<System.InvalidOperationException>(fun()->
//        q.enqueue('2')
//        )

//    //output.WriteLine(ex.Message)
//    Should.equal ex.Message "should tryNext first."

//[<Fact>]
//member this.``05 dequeque right test``() =
//    let q = BufferResizeArray()
//    q.enqueue(1)
//    q.enqueue(2)

//    q.dequeue(2)
//    Should.equal 0 q.count
//    Should.equal -1 q.current

//[<Fact>]
//member this.``06 dequeque empty test``() =
//    let q = BufferResizeArray()

//    let ex = Assert.Throws<System.InvalidOperationException>(fun()->
//        q.dequeue(2)
//        )
//    //output.WriteLine(ex.Message)
//    Should.equal ex.Message "should count <= current + 1"

//[<Fact>]
//member this.``07 dequeque error test``() =
//    let q = BufferResizeArray()
//    q.enqueue('1')

//    let ex = Assert.Throws<System.InvalidOperationException>(fun()->
//        q.dequeue(2)
//        )
//    //output.WriteLine(ex.Message)
//    Should.equal ex.Message "should count <= current + 1"
