namespace FSharp.Idioms

open Xunit

open FSharp.xUnit
open System

type ArrayStackTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``tryNext test``() =
        let xs = [ 'a'; 'b' ] |> Array.ofSeq
        let iterator = ArrayStack(xs)
        let ys = [
            iterator.tryNext ()
            iterator.tryNext ()
            iterator.tryNext ()
            iterator.tryNext ()
        ]
        let es = [ Some 'a'; Some 'b'; None; None ]
        Should.equal es ys

    [<Fact>]
    member this.``reset Iterator``() =
        let iterable = [ 1..5 ] |> Array.ofSeq
        let iterator = ArrayStack(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.reset ()
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value //测试衔接是否正确
        ]
        let e = [ 1; 2; 1; 2; 3 ]
        Should.equal y e

    [<Fact>]
    member this.``pop 1 Iterator``() =
        let iterable = [ 1..5 ] |> Array.ofSeq
        let iterator = ArrayStack(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.pop (1)
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 2; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``pop Iterator``() =
        let iterable = [ 1..5 ] |> Array.ofSeq
        let iterator = ArrayStack(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.pop (2)
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 3; 4; 5 ]
        Should.equal y e

    [<Fact>]
    member this.``pop count Iterator``() =
        let iterable = [ 1..5 ] |> Array.ofSeq
        let iterator = ArrayStack(iterable)
        let y = [
            iterator.tryNext().Value
            iterator.tryNext().Value
            iterator.tryNext().Value
            do iterator.pop (2)
            iterator.tryNext().Value
            iterator.tryNext().Value
        ]
        let e = [ 1; 2; 3; 3; 4 ]
        Should.equal y e

    [<Fact>]
    member this.``pop count``() =
        let arr = [| 1..5 |]
        let stack = ArrayStack(arr)

        let a = stack.tryNext()
        let b = stack.tryNext()
        stack.pop (2)

    
    [<Fact>]
    member _.``pop with negative count should throw ArgumentException``() =
        let stack = ArrayStack([|1; 2; 3|])
        Assert.Throws<ArgumentException>(fun () -> stack.pop(-1) |> ignore)
    
    [<Fact>]
    member _.``pop count greater than current position should throw``() =
        let stack = ArrayStack([|1; 2; 3|])
        // 移动到第一个元素 (iCurrent = 0)
        stack.tryNext() |> ignore
        // 尝试弹出2个元素，但只看到了1个
        Assert.Throws<ArgumentException>(fun () -> stack.pop(2) |> ignore)
    
    [<Fact>]
    member _.``pop count greater than total count should throw``() =
        let stack = ArrayStack([|1; 2|])
        // 移动到第二个元素 (iCurrent = 1)
        stack.tryNext() |> ignore // 移动到1
        stack.tryNext() |> ignore // 移动到2
        // 尝试弹出3个元素，但总共只有2个
        Assert.Throws<ArgumentException>(fun () -> stack.pop(3) |> ignore)
    
    [<Fact>]
    member _.``pop when not started (iCurrent = -1) should throw for any positive count``() =
        let stack = ArrayStack([|1; 2; 3|])
        // iCurrent = -1，还没有开始迭代
        let ex = Assert.Throws<ArgumentException>(fun () -> stack.pop(1) |> ignore)
        let ex = Assert.Throws<ArgumentException>(fun () -> stack.pop(2) |> ignore)
        ()
    
    [<Fact>]
    member _.``pop when iteration completed (iCurrent = count) should throw for count > 0``() =
        let stack = ArrayStack([|1; 2|])
        // 完成迭代
        Should.equal (stack.tryNext()) (Some 1)
        Should.equal (stack.tryNext()) (Some 2)
        Should.equal (stack.tryNext()) (None)
        //Should.equal stack.current 2
        Should.equal stack.count 2
        
        // 可以弹出0个元素（无操作）
        stack.pop(0) // 应该不会抛出异常
        
        // 但不能弹出任何正数
        let ex = Assert.Throws<ArgumentException>(fun () -> stack.pop(1) |> ignore)
        let ex = Assert.Throws<ArgumentException>(fun () -> stack.pop(2) |> ignore)
        ()
    [<Fact>]
    member _.``pop zero count should be allowed in any state``() =
        let stack = ArrayStack([|1; 2; 3|])
        // 初始状态
        stack.pop(0) // 应该不会抛出异常
        
        // 移动后
        Should.equal (stack.tryNext()) (Some 1)

        stack.pop(0) // 应该不会抛出异常
        
        // 迭代完成后
        Should.equal (stack.tryNext()) (Some 1)
        Should.equal (stack.tryNext()) (Some 2)
        Should.equal (stack.tryNext()) (Some 3)

        stack.pop(0) // 应该不会抛出异常

        Should.equal (stack.tryNext()) (Some 1)
        Should.equal (stack.tryNext()) (Some 2)
        Should.equal (stack.tryNext()) (Some 3)
        Should.equal (stack.tryNext()) (None)
        Should.equal (stack.tryNext()) (None)
        Should.equal (stack.tryNext()) (None)
        Should.equal (stack.tryNext()) (None)

        stack.pop(0) // 应该不会抛出异常
    
    [<Fact>]
    member _.``valid pop scenarios should work correctly``() =
        let stack = ArrayStack([|1; 2; 3; 4; 5|])
    
        // 移动到第一个元素
        Assert.Equal(Some 1, stack.tryNext())
        // 弹出1个看到的元素
        stack.pop(1)
        // 现在应该从2开始
        Assert.Equal(Some 2, stack.tryNext())
    
        // 再移动两个
        Assert.Equal(Some 3, stack.tryNext())
        Assert.Equal(Some 4, stack.tryNext())
        // 弹出2个看到的元素
        stack.pop(2)
        // 现在应该从4开始（因为弹出2个后，iStart=3，数组[4,5]）
        Assert.Equal(Some 4, stack.tryNext())  // 修正：应该是4而不是5
        Assert.Equal(Some 5, stack.tryNext())
        Assert.Equal(None, stack.tryNext())
    
    [<Fact>]
    member _.``pop exactly current position + 1 should work``() =
        let stack = ArrayStack([|1; 2; 3|])
        
        // 移动到第二个元素 (iCurrent = 1)
        stack.tryNext() |> ignore // 1
        stack.tryNext() |> ignore // 2
        
        // 弹出正好看到的元素数量 (2)
        stack.pop(2)
        
        // 现在应该从3开始
        Assert.Equal(Some 3, stack.tryNext())
        Assert.Equal(None, stack.tryNext())
    
    [<Fact>]
    member _.``pop less than current position should reset to correct position``() =
        let stack = ArrayStack([|1; 2; 3; 4; 5|])
        
        // 移动到第四个元素 (iCurrent = 3)
        stack.tryNext() |> ignore // 1
        stack.tryNext() |> ignore // 2
        stack.tryNext() |> ignore // 3
        stack.tryNext() |> ignore // 4
        
        // 只弹出2个元素
        stack.pop(2)
        
        // 应该从3开始，并且重置到起始前
        Assert.Equal(Some 3, stack.tryNext()) // 第一个元素是3
        Assert.Equal(Some 4, stack.tryNext()) // 第二个是4
        Assert.Equal(Some 5, stack.tryNext()) // 第三个是5
        Assert.Equal(None, stack.tryNext())

    [<Fact>]
    member _.``用current指针判断迭代到结尾``() =
        let arr = [|1; 2; 3;|]
        let stack = ArrayStack(arr)

        //没有执行pop，count会保持不变
        Should.equal stack.count 3
        //Should.equal stack.current -1
        //Assert.True(stack.current < stack.count)

        Assert.Equal(Some 1, stack.tryNext())
        //Should.equal stack.current 0
        Should.equal stack.count 3
        //Assert.True(stack.current < stack.count)

        Assert.Equal(Some 2, stack.tryNext())
        //Should.equal stack.current 1
        Should.equal stack.count 3
        //Assert.True(stack.current < stack.count)

        Assert.Equal(Some 3, stack.tryNext())
        //Should.equal stack.current 2
        Should.equal stack.count 3
        //Assert.True(stack.current < stack.count)

        //重复任意多次下一个
        Assert.Equal(None, stack.tryNext())
        //Should.equal stack.current 3
        Should.equal stack.count 3
        //Assert.True(stack.current = stack.count)

        Assert.Equal(None, stack.tryNext())
        //Should.equal stack.current 3
        Should.equal stack.count 3
        //Assert.True(stack.current = stack.count)

        Assert.Equal(None, stack.tryNext())
        //Should.equal stack.current 3
        Should.equal stack.count 3
        //Assert.True(stack.current = stack.count)

