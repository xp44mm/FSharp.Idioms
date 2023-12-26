module FSharp.Idioms.OptionType

open System
open System.Collections.Concurrent

let get_Value = 
    // typeof<option<'v>>
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let m = ty.GetMethod("get_Value")
        fun (obj:obj) -> m.Invoke(obj,[||])
    )
