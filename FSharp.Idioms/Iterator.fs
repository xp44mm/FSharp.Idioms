namespace FSharp.Idioms

open System.Collections.Generic

type Iterator<'a>(enumerator:IEnumerator<'a>) =
    let mutable hasDone = false

    member _.tryNext() =
        if hasDone then
            None
        else
            do hasDone <- not(enumerator.MoveNext())
            if hasDone then
                None
            else
                Some enumerator.Current

    member this.toSeq() =
        let rec loop () =
            seq {
                match this.tryNext() with
                | None -> ()
                | Some value -> 
                    yield value
                    yield! loop ()
            }
        loop ()