module FSharp.Idioms.IEnumerableType

open System.Collections

let IEnumerableType = typeof<IEnumerable>

let GetEnumerator  = IEnumerableType.GetMethod("GetEnumerator")

let getEnumerator (x:obj) = 
    GetEnumerator.Invoke(x,[||])
    :?> IEnumerator

let toArray (value:obj) =
    let enm = getEnumerator value
    [|
        while enm.MoveNext() do
            yield enm.Current
    |]

let toList (value:obj) =
    let enm = getEnumerator value
    [
        while enm.MoveNext() do
            yield enm.Current
    ]

