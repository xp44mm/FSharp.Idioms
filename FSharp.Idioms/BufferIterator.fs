namespace FSharp.Idioms

open System.Collections.Generic
open System

///自动缓存队列，dequeque出队后不再缓存
type BufferIterator<'a>(source: IEnumerator<'a>) =
    // 本类型的当前位置
    let mutable _current = None

    //读取到，还没有用dequeue消耗掉的数据，每次消耗后，重新从缓存中的头部读取数据。
    let buffer = BufferResizeArray<'a>()

    let iterator = Iterator(source)

    /// 试探性地向前移动指针，向队尾方向，超出队则读取源序列，然后返回当前值。
    member _.tryNext() =
        _current <-
            // 先读取缓存队列，再读取数据来源
            match buffer.tryNext () with
            | Some e -> Some e
            | None ->
                match iterator.tryNext () with
                | None -> None
                | maybe ->
                    buffer.enqueue (maybe.Value)
                    maybe

        _current

    [<Obsolete("let mutalbe mobile = true")>]
    member _.ongoing() =
        (buffer.count > 0)
        || iterator.current.IsSome
        || (try
                let _ = source.Current
                true
            with ex ->
                //枚举尚未开始。请调用 MoveNext。
                //枚举已完成。
                ex.Message.Contains "MoveNext")

    member this.current = _current

    /// 确认消费出队count个元素，使tryNext指向队头。
    /// 从上一次dequeue的位置开始dequeue
    member _.dequeue(count) = buffer.dequeue (count)

    /// 确认出队1个元素，使tryNext指向队头
    [<Obsolete("this.dequeue(1)")>]
    member this.dequeueHead() = this.dequeue (1)

    /// 确认不消费任何元素，使tryNext指向队头
    [<Obsolete("this.dequeue(0)")>]
    member this.dequequeNothing() = this.dequeue (0)

    /// 确认出队自上次确认以来的所有tryNext出来的元素，使tryNext指向队头
    [<Obsolete("this.dequeue(count)")>]
    member this.dequeueToCurrent() =
        this.dequeue (buffer.current + 1)

    new(source: seq<_>) = BufferIterator<_>(source.GetEnumerator())
