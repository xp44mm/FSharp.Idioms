module FSharp.Idioms.Reflection.MapType

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection

open FSharp.Reflection

let mapModuleType = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.MapModule")
let mToArrayDef = mapModuleType.GetMethod "ToArray"
let mOfArrayDef = mapModuleType.GetMethod "OfArray"

//let memoToArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

//let getToArray (mapType:Type) =
//    let valueFactory (ty:Type) =
//        mToArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
//    memoToArray.GetOrAdd(mapType, valueFactory)

//let memoOfArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

//let getOfArray (mapType:Type) =
//    let valueFactory (ty:Type) =
//        mOfArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
//    memoOfArray.GetOrAdd(mapType, valueFactory)

let empty =
    let memo = ConcurrentDictionary<Type, obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ctor = ty.GetConstructors().[0]
        let ety = 
            ctor.GetParameters().[0]
                .ParameterType
                .GenericTypeArguments.[0]
        let arr = Array.CreateInstance(ety, 0)
        ctor.Invoke [| arr |]
    )

let toArray (mty:Type) =
    // KeyValuePair<'k,'v>
    let kvty = 
        mty.GetInterface("IEnumerable`1").GenericTypeArguments.[0]
            
    let get_Key = KeyValuePairType.get_Key kvty
    let get_Value = KeyValuePairType.get_Value kvty

    fun (mp:obj) ->
        mp
        |> IEnumerableType.toArray
        |> Array.map(fun kvp -> get_Key kvp, get_Value kvp )


