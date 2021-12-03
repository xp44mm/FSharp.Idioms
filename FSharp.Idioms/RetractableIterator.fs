namespace FSharp.Idioms

open System.Collections.Generic

type RetractableIterator<'a>(enumerator:IEnumerator<'a>) =
    let iterator = Iterator(enumerator)
    let values = ResizeArray<'a>()
    let mutable hasDone = false
    let mutable forward = -1

    member this.allDone() =
        hasDone && values.Count = 0 && forward = -1

    member this.dequeue(count) =
        do forward <- -1
        let hd = values.GetRange(0,count).ToArray()
        do values.RemoveRange(0,count)
        hd

    member this.tryNext() =
        if forward < values.Count - 1 then
            forward <- forward + 1
            Some values.[forward]
        elif hasDone then
            None
        else
            let maybeE = iterator.tryNext()

            match maybeE with
            | Some value ->
                values.Add(value)
                forward <- forward + 1
            | None as e ->
                hasDone <- true

            maybeE
