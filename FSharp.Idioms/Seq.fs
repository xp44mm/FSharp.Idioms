﻿[<RequireQualifiedAccess>]
module FSharp.Idioms.Seq

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

let rec makeSeq tryNext =
    seq {
        match tryNext() with
        | None -> ()
        | Some x -> 
            yield x
            yield! makeSeq tryNext
    }
