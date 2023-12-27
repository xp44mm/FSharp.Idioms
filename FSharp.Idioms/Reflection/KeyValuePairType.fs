module FSharp.Idioms.Reflection.KeyValuePairType

open System
open System.Collections.Concurrent

let get_Key = 
    // typeof<KeyValuePair<'k,'v>>
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let m = ty.GetMethod("get_Key")
        fun (kvp:obj) -> m.Invoke(kvp,[||])
    )
let get_Value = 
    // typeof<KeyValuePair<'k,'v>>
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let m = ty.GetMethod("get_Value")
        fun (kvp:obj) -> m.Invoke(kvp,[||])
    )
