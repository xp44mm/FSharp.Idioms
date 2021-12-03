namespace FSharp.Idioms

open System.Collections.Generic

type Iterator<'a>(enumerator:IEnumerator<'a>) =
    let mutable hasDone = false
    member this.tryNext() =
        if hasDone then
            None
        else
            do hasDone <- not(enumerator.MoveNext())
            if hasDone then
                None
            else
                Some enumerator.Current
