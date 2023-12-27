module FSharp.Idioms.Reflection.ArrayType

open System
open System.Collections.Concurrent
open System.Reflection

let ArrayType = typeof<System.Array>
let get_Length = ArrayType.GetMethod("get_Length")

let length (value:obj) =
    get_Length.Invoke(value,[||])
    :?> int

let GetValue = ArrayType.GetMethod("GetValue",[|typeof<int>|])

let getValue (i:int) (value:obj) =
    GetValue.Invoke(value,[|i|])

let toArray (arr:obj) =
    arr
    |> length
    |> Array.create <| 0
    |> Array.mapi(fun i _ -> getValue i arr)

