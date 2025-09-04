namespace FSharp.Idioms

open System
open System.Collections.Generic

type Iterator<'a>(enumerator: IEnumerator<'a>) =
    new(sq: seq<_>) = Iterator<_>(sq.GetEnumerator())
    member _.tryNext() =
        if enumerator.MoveNext() then
            Some enumerator.Current
        else
            None
