namespace FSharp.Idioms

open System
open System.Collections.Generic

type Iterator<'a>(enumerator:IEnumerator<'a>) =
    let mutable moveNext = true

    new(sq:seq<_>) = Iterator<_>(sq.GetEnumerator())

    member _.tryNext() =
        if moveNext then
            if enumerator.MoveNext() then
                Some enumerator.Current
            else
                moveNext <- false
                None
        else
            None

    member _.ongoing() = moveNext

    /// rest to seq
    [<Obsolete("Seq.make this.tryNext")>]
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
