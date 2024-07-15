namespace FSharp.Idioms

open System.Collections.Generic
open System

///自动缓存队列，dequeque出队后不再缓存
type BufferIterator<'a>(source:IEnumerator<'a>) =
    //指示数据来源是否能向前移动
    let mutable moveNext = true

    //读取到，还没有用dequeue消耗掉的数据，每次消耗后，重新从缓存中的头部读取数据。
    let q = BufferQueue<'a>()

    new(source:seq<_>) = BufferIterator<_>(source.GetEnumerator())

    /// 试探性地向前移动指针，向队尾方向，超出队则读取源序列，然后返回当前值。
    member _.tryNext() =
        // 先读取缓存队列，再读取数据来源
        match q.tryNext() with
        | Some e -> Some e
        | None -> 
            if moveNext then
                if source.MoveNext() then
                    let value = source.Current
                    q.enqueue(value)
                    Some value
                else
                    moveNext <- false
                    None
            else
                None

    member _.ongoing() =
        moveNext || not q.isEmpty

    /// 确认消费出队count个元素，使tryNext指向队头
    member _.dequeue(count) = 
        q.dequeque(count)

    /// 确认不消费任何元素，使tryNext指向队头
    member _.dequequeNothing() = 
        q.dequequeNothing()

    /// 确认出队自上次确认以来的所有tryNext出来的元素，使tryNext指向队头
    member this.dequeueToCurrent() = 
        q.dequeueToCurrent()

    /// 确认出队1个元素，使tryNext指向队头
    member this.dequeueHead() = 
        q.dequeque(1)



