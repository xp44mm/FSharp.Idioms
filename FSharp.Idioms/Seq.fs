[<RequireQualifiedAccess>]
module FSharp.Idioms.Seq
open System

let tap f (source:'a seq) =
    source
    |> Seq.map(fun  x -> 
        f x
        x
    )

let tapi f (source:'a seq) =
    source
    |> Seq.mapi(fun i x -> 
        f i x
        x
    )

let rec make tryNext =
    seq {
        match tryNext() with
        | None -> ()
        | Some x -> 
            yield x
            yield! make tryNext
    }

[<Obsolete("Seq.make")>]
let rec makeSeq tryNext = make tryNext
