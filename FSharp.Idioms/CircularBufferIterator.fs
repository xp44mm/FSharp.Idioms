namespace FSharp.Idioms

open System
open System.Collections.Generic

/// 缓存迭代器，容量99
type CircularBufferIterator<'a>(source: IEnumerator<'a>) =
    let iterator = Iterator(source)
    let buffer = CircularBuffer<'a>(100)

    new(source: seq<_>) = CircularBufferIterator<_>(source.GetEnumerator())

    /// 缓存到尾部的时候，读取源序列，读取完序列，缓存移动到尾部
    member _.tryNext() =
        match buffer.tryNext() with
        | None ->
            match iterator.tryNext() with
            | None -> None
            | (Some item) as value ->
                buffer.enqueue(item)
                value
        | Some item -> Some item

    member _.dequeue(count) = buffer.dequeue(count)

    ///当前指针恢复到头部
    member this.reset() = buffer.reset()
