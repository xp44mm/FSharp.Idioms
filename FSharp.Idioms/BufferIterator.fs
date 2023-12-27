namespace FSharp.Idioms

open System.Collections.Generic
open System

///此类型维持一个缓存队列，可以试探性地向前迭代数据元素。
type BufferIterator<'a>(enumerator:IEnumerator<'a>) =
    let mutable moveNext = true

    //读取到，还没有用dequeue消耗掉的数据，每次消耗后，重新从缓存中的头部读取数据。
    let q = BufferQueue<'a>()

    new(sq:seq<_>) = BufferIterator<_>(sq.GetEnumerator())

    /// 试探性地向前移动指针，向队尾方向，超出队则读取源序列，然后返回当前值。
    member _.tryNext() =
        match q.tryNext() with
        | Some e -> Some e
        | None -> 
            if moveNext then
                if enumerator.MoveNext() then
                    let value = enumerator.Current
                    q.enqueue(value)
                    Some value
                else
                    moveNext <- false
                    None
            else
                None

    member _.ongoing() =
        moveNext || not q.isEmpty

    /// 出队count个元素，缓存剩余的元素，仿佛它未读取。
    member _.dequeue(count) = 
        q.dequeque(count)

    /// 出队0个元素，缓存剩余的元素，仿佛它未读取。
    member _.dequequeNothing() = 
        q.dequequeNothing()

    /// 出队已经读取的元素，缓存剩余的元素，仿佛它未读取。
    member this.dequeueToCurrent() = 
        q.dequeueToCurrent()

    /// 出队1个元素，缓存剩余的元素，仿佛它未读取。
    member this.dequeueHead() = 
        q.dequeque(1)



