namespace FSharp.Idioms

open System
open System.Collections.Generic

type Iterator<'a>(enumerator: IEnumerator<'a>) =
    let mutable _current = None

    member _.tryNext() =
        _current <-
            if enumerator.MoveNext() then
                Some enumerator.Current
            else
                None
        _current

    member _.current = _current

    new(sq: seq<_>) = Iterator<_>(sq.GetEnumerator())
