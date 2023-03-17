namespace FSharp.Idioms

open System.Collections.Generic
open System

///类型`RetractableIterator`维持一个缓存队列，可以试探性地向前迭代数据元素。
[<Obsolete("=> BufferIterator")>]
type RetractableIterator<'a>(enumerator:IEnumerator<'a>) =
    let mutable moveNext = true

    //读取到，还没有用dequeue消耗掉的数据，每次消耗后，重新从缓存中的头部读取数据。
    let bufferQueue = ResizeArray<'a>()

    //当前值索引:tryNext的位置。
    let mutable forward = -1

    new(sq:seq<_>) = RetractableIterator<_>(sq.GetEnumerator())

    /// 试探性地向前移动指针，向队尾方向，超出队则读取源序列，然后返回当前值。
    member _.tryNext() =
        if forward < bufferQueue.Count - 1 then
            //说明forward在缓存队中
            //读取缓存中的值，将当前值索引前进一位
            forward <- forward + 1
            Some bufferQueue.[forward]

        elif moveNext then
            if enumerator.MoveNext() then
                //取到值
                let value = enumerator.Current
                //添加进入缓存队
                bufferQueue.Add(value)
                forward <- forward + 1

                Some value
            else
                moveNext <- false
                None
        else
            None

    member _.ongoing() =
        moveNext || bufferQueue.Count > 0

    /// 出队count个元素，指针置于队头
    member _.dequeue(count) =
        if count > bufferQueue.Count then
            failwith $"out of bounds:{count}>{bufferQueue.Count}"

        //再次调用tryNext()时，从缓存头部开始读取。
        forward <- -1
        // 从缓存头部消耗掉count个数据。
        let hd = bufferQueue.GetRange(0,count).ToArray()
        bufferQueue.RemoveRange(0,count)
        hd

    /// 出队1个元素，指针置于队头
    member this.dequeueHead()=
        this.dequeue(1)
        |> Seq.head

    /// 出队0个元素，指针置于队头
    member _.dequequeNothing() =
        forward <- -1

    member this.dequeueToCurrent() = this.dequeue(forward + 1)

    member this.dequeueBuffer() =
        this.dequeue(bufferQueue.Count)



    [<Obsolete("this.dequequeNothing()")>]
    member _.restart() =
        forward <- -1

    ///确定从预读的数据中消费一个数据
    [<Obsolete("this.dequeueHead()")>]
    member this.consume() =
        this.dequeue(1) |> Seq.head

    ///确定预读的数据被消费，按照给定的数量。
    [<Obsolete("this.dequeue(count)")>]
    member this.consume(count) =
        this.dequeue(count)

    /// rest to seq include buffer
    [<Obsolete("Seq.make this.tryNext")>]
    member this.toSeq() =
        this.dequequeNothing()
        let rec loop () =
            seq {
                match this.tryNext() with
                | None -> ()
                | Some value ->
                    yield this.dequeueHead()
                    yield! loop ()
            }
        loop ()

    ///全部消费完成
    [<Obsolete("not(this.ongoing())")>]
    member _.allDone() =
         not moveNext && bufferQueue.Count = 0 // && forward = -1
