module FSharp.Idioms.Jsons.JsonCapture

open FSharp.Idioms
open FSharp.Idioms.Jsons.JsonWriters
open FSharp.Reflection

open System
open System.Collections.Generic

/// 找到记录中第一个属性
let tryCapture (field:string) (records:Json list) =
    records
    |> List.tryPick(fun h ->
        if h.hasProperty field then
            Some h.[field]
        else None
    )
