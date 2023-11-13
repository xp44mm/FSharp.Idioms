module FSharp.Idioms.IEnumerableType

open System.Collections

let EnumerableType = typeof<IEnumerable>

let GetEnumerator  = EnumerableType.GetMethod("GetEnumerator")

let getEnumerator (x:obj) = 
    GetEnumerator.Invoke(x,[||])
    :?> IEnumerator

let toArray (value:obj) =
    let enm = getEnumerator value
    [|
        while enm.MoveNext() do
            yield enm.Current
    |]

