namespace FSharp.Idioms

open System

///带当前指针的环形队
type CircularBuffer<'T>(buffer: 'T[]) =
    let mutable front = 0
    let mutable rear = 0
    let mutable current = 0

    let future c i =
        let y = i + c
        if y >= buffer.Length then
            y - buffer.Length
        else
            y

    let tomorrow i = future 1 i

    let leng rear front =
        let y = rear - front
        if y >= 0 then y else y + buffer.Length
        
    member this.count = leng rear front

    member this.isempty = front = rear

    ///容量比背后数组长度少贰
    member this.isfull = tomorrow rear = front

    member this.tryNext() =
        if current = rear then
            None
        else
            let item = Some buffer.[current]
            current <- tomorrow current
            item

    member this.enqueue(item: 'T) =
        if this.isfull then
            failwith "CircularBuffer is full"
        else
            if current = rear then
                buffer.[rear] <- item
                rear <- tomorrow rear
                current <- rear
            else failwith "当前位置必须在末尾"

    member this.dequeue(count) =
        if count < 0 then
            raise(ArgumentException "Count must be non-negative")
        elif count > leng current front then
            raise(ArgumentException $"只能弹出已经看到的元素:{leng current front}")

        front <- future count front
        this.reset()

    /// current <- front
    member this.reset() = current <- front

    new(count)
        =
        let arr = Array.zeroCreate<_> count
        CircularBuffer(arr)
