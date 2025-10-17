namespace FSharp.Idioms

open System

type ArrayStack<'T>(items: 'T array) =
    let mutable start = 0
    let mutable current = 0

    member _.count = items.Length - start
    member _.isEmpty = items.Length = start

    /// 尝试移动到下一个元素
    member this.tryNext() =
        if current = items.Length then
            None
        else
            let item = Some items.[current]
            current <- current + 1
            item

    /// 弹出指定数量的元素
    member this.pop(count) =
        if count < 0 then
            raise(ArgumentException "Count must be non-negative")
        elif count > current - start then
            raise(ArgumentException "只能弹出已经看到的元素")
        start <- start + count
        this.reset()

    /// current <- start
    member this.reset() = current <- start
