module FSharp.Idioms.Reflection.SetType

open System
open System.Collections.Concurrent
open System.Reflection

let setModuleType  = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.SetModule")

let mToArrayDef = setModuleType.GetMethod "ToArray"
let mOfArrayDef = setModuleType.GetMethod "OfArray"

let memoToArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

let getToArray (setType:Type) =
    let valueFactory (ty:Type) =
        mToArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
    memoToArray.GetOrAdd(setType, valueFactory)

let memoOfArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

let getOfArray (setType:Type) =
    let valueFactory (ty:Type) =
        mOfArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
    memoOfArray.GetOrAdd(setType, valueFactory)

let ctor =
    let memo = ConcurrentDictionary<Type, obj->obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0],
        let ctor = ty.GetConstructors().[0]
        fun sq -> ctor.Invoke([|sq|])
    )

let empty =
    let memo = ConcurrentDictionary<Type, obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty.GenericTypeArguments.[0],
        let arr = Array.CreateInstance(ty.GenericTypeArguments.[0], 0)
        ctor ty arr
    )
