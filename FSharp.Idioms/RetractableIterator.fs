namespace FSharp.Idioms

open System.Collections.Generic

type RetractableIterator<'a>(enumerator:IEnumerator<'a>) =
    let iterator = Iterator(enumerator)
    //读取到，还没有用dequeue消耗掉的数据，每次消耗后，重新从缓存中的头部读取数据。
    let values = ResizeArray<'a>()
    //当前值索引:tryNext的位置。
    let mutable forward = -1
    //iterator是否已经读取完了
    let mutable hasDone = false

    member _.allDone() =
        hasDone && values.Count = 0 && forward = -1

    member _.restart() = 
        forward <- -1

    member this.consume() =
        this.dequeue(1)
        |> Seq.head

    member this.consume(count) =
        this.dequeue(count)

    /// 从缓存头部消耗掉count个数据。
    member private _.dequeue(count) =
        // 从缓存头部消耗掉count个数据。
        let hd = values.GetRange(0,count).ToArray()
        values.RemoveRange(0,count)
        //再次调用tryNext()时，从缓存头部开始读取。
        forward <- -1
        hd

    /// 向前读取数据，被读取的数据仍在缓存中，并没有被消耗。
    member _.tryNext() =
        if forward < values.Count - 1 then
            //说明forward在缓存values中，
            //读取缓存中的值，将当前值索引前进一位
            forward <- forward + 1
            Some values.[forward]
        elif hasDone then
            None
        else
            let maybeE = iterator.tryNext()

            match maybeE with
            | Some value ->
                //取到值后就添加进入缓存的末尾
                values.Add(value)
                //forward == values.count
                forward <- forward + 1
            | None ->
                // 更新iterator的状态，已经遍历完成。
                hasDone <- true

            maybeE
