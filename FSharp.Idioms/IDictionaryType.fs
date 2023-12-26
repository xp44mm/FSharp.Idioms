module FSharp.Idioms.IDictionaryType

open System.Collections
open System

let get_Keys = fun (ty:Type) ->
    match ty.GetInterface("IDictionary`2") with
    | null -> failwith ""
    | dty ->
    let get_Keys = dty.GetMethod("get_Keys")
    fun (mp:obj) ->
    let keysObj = get_Keys.Invoke(mp,[||])
    IEnumerableType.toList keysObj
