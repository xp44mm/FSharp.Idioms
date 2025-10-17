namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit
open System

type CircularBufferTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``tryNext test``() =        
        let buffer = CircularBuffer(100)
        let ys = [
            buffer.tryNext()
            buffer.tryNext()
            buffer.tryNext()
            buffer.tryNext()
        ]
        // 空缓冲区的tryNext应该都返回None
        for y in ys do
            Should.equal None y

    [<Fact>]
    member this.``basic enqueue and dequeue``() =
        let buffer = CircularBuffer<int>(3)
        
        buffer.enqueue(1)
        buffer.enqueue(2)
        
        // 缓冲区已满
        Assert.True(buffer.isfull)
        Assert.Equal(2, buffer.count)
        
        buffer.dequeue(1)
        buffer.tryNext() |> Should.equal (Some 2)
        
        buffer.enqueue(4)

        buffer.reset()
        buffer.tryNext() |> Should.equal (Some 2)
        buffer.tryNext() |> Should.equal (Some 4)


    [<Fact>]
    member this.``empty buffer properties``() =
        let arr = Array.zeroCreate<int> 5        
        let buffer = CircularBuffer(arr)
        
        Assert.True(buffer.isempty)
        Assert.False(buffer.isfull)
        Assert.Equal(0, buffer.count)
        Assert.Equal(5, arr.Length)

    [<Fact>]
    member this.``full buffer test``() =
        let buffer = CircularBuffer<int>(3)
        
        buffer.enqueue(10)
        buffer.enqueue(20)
        
        Assert.True(buffer.isfull)
        Assert.False(buffer.isempty)
        Assert.Equal(2, buffer.count)
        
        // 尝试添加第三个元素应该抛出异常
        let ex = Assert.Throws<Exception> (fun () -> buffer.enqueue(30))
        output.WriteLine(ex.Message)

    [<Fact>]
    member this.``circular behavior test``() =
        let buffer = CircularBuffer<int>(4)
        
        // 填充缓冲区
        buffer.enqueue(1)
        buffer.enqueue(2)
        buffer.enqueue(3)
        
        // 出队一个元素
        buffer.dequeue(1)

        buffer.tryNext() |> Should.equal (Some 2)
        buffer.tryNext() |> Should.equal (Some 3)
        
        // 应该可以继续添加
        buffer.enqueue(4)
        
        Assert.Equal(3, buffer.count)
        Assert.True(buffer.isfull)

    [<Fact>]
    member this.``multiple dequeue test``() =
        let buffer = CircularBuffer<int>(7)
        
        for i in 1..5 do
            buffer.enqueue(i)

        buffer.dequeue(3) // 出队前3个元素

        buffer.tryNext() |> Should.equal (Some 4)
        buffer.tryNext() |> Should.equal (Some 5)
        Assert.Equal(2, buffer.count)
        
        buffer.enqueue(6)
        buffer.enqueue(7)
        
        Assert.Equal(4, buffer.count)
        Assert.False(buffer.isfull)

    [<Fact>]
    member this.``tryNext with data``() =
        let buffer = CircularBuffer<int>(5)
        
        buffer.enqueue(1)
        buffer.enqueue(2)
        buffer.enqueue(3)
        
        buffer.reset()

        // 重置current指针到front之前
        // 注意：根据您的实现，tryNext从current开始，需要确保current在正确位置
        let results = [
            buffer.tryNext()
            buffer.tryNext()
            buffer.tryNext()
            buffer.tryNext() // 第四次应该返回None
        ]
        
        let expected = [Some 1; Some 2; Some 3; None]
        Should.equal expected results

    [<Fact>]
    member this.``edge case - single element buffer``() =
        let buffer = CircularBuffer<int>(2)
        
        buffer.enqueue(42)
        Assert.True(buffer.isfull)
        Assert.Equal(1, buffer.count)
        
        //buffer.tryNext() |> Should.equal (Some 42)

        buffer.dequeue(1)

        Assert.True(buffer.isempty)
        
        buffer.enqueue(99)
        Assert.True(buffer.isfull)

