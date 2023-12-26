module FSharp.Idioms.IEnumeratorType

open System.Collections

// interfaces
let EnumeratorType = typeof<IEnumerator>

// members
let MoveNext = EnumeratorType.GetMethod("MoveNext")
let Current = EnumeratorType.GetProperty("Current")

// apps
let moveNext (enumerator:obj) = 
    MoveNext.Invoke(enumerator,[||])
    :?> bool

let current (enumerator:obj) = 
    Current.GetValue(enumerator)

let tryNext(enumerator:IEnumerator) =
    if enumerator.MoveNext() then
        enumerator.Current |> Some
    else
        None

let toArray (enumerator:IEnumerator) =
    [|
        while enumerator.MoveNext() do
            yield enumerator.Current
    |]
