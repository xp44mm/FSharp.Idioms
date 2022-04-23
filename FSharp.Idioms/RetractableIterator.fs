namespace FSharp.Idioms

open System.Collections.Generic

type RetractableIterator<'a>(enumerator:IEnumerator<'a>) =
    let iterator = Iterator(enumerator)
    let values = ResizeArray<'a>()
    //当前值索引
    let mutable forward = -1
    //iterator是否已经读取完了
    let mutable hasDone = false

    member _.allDone() =
        hasDone && values.Count = 0 && forward = -1

    member _.dequeue(count) =
        forward <- -1
        let hd = values.GetRange(0,count).ToArray()
        values.RemoveRange(0,count)
        hd

    member _.tryNext() =
        if forward < values.Count - 1 then
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
                forward <- forward + 1
            | None ->
                // 更新iterator的状态，已经遍历完成。
                hasDone <- true

            maybeE
