[<RequireQualifiedAccess>]
module FSharp.Idioms.Seq

open System
open System.IO

let tap f (source: 'a seq) =
    source
    |> Seq.map (fun x ->
        f x
        x
    )

let tapi f (source: 'a seq) =
    source
    |> Seq.mapi (fun i x ->
        f i x
        x
    )

let rec make tryNext = seq {
    match tryNext () with
    | None -> ()
    | Some x ->
        yield x
        yield! make tryNext
}

[<Obsolete("FSharp.Idioms.Seq.make")>]
let rec makeSeq tryNext = make tryNext

let fromTextReader (reader: TextReader) : seq<char> = seq {
    let mutable current = reader.Read()
    while current <> -1 do
        yield char current
        current <- reader.Read()
}
