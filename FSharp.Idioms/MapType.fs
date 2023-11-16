module FSharp.Idioms.MapType

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection

open Microsoft.FSharp.Reflection

let mapModuleType = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.MapModule")
let mToArrayDef = mapModuleType.GetMethod "ToArray"
let mOfArrayDef = mapModuleType.GetMethod "OfArray"

let memoToArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

let getToArray (mapType:Type) =
    let valueFactory (ty:Type) =
        mToArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
    memoToArray.GetOrAdd(mapType, valueFactory)

let memoOfArray = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)

let getOfArray (mapType:Type) =
    let valueFactory (ty:Type) =
        mOfArrayDef.MakeGenericMethod(ty.GenericTypeArguments)
    memoOfArray.GetOrAdd(mapType, valueFactory)

let empty =
    let memo = ConcurrentDictionary<Type, obj>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ctor = ty.GetConstructors().[0]
        // Tuple`2<_,_>
        let ety = 
            ctor.GetParameters().[0]
                .ParameterType
                .GenericTypeArguments.[0]
        let arr = Array.CreateInstance(ety, 0)
        ctor.Invoke([| arr |])
    )

let toArray (mty:Type) =
    let kty = 
        typeof<KeyValuePair<_,_>>
            .GetGenericTypeDefinition()
            .MakeGenericType(mty.GenericTypeArguments)
    
    let get_Key = kty.GetMethod("get_Key")
    let key (kvp:obj) = get_Key.Invoke(kvp,[||])

    let get_Value = kty.GetMethod("get_Value")
    let value (kvp:obj) = get_Value.Invoke(kvp,[||])

    fun (mp:obj) ->
        mp
        |> IEnumerableType.toArray
        |> Array.map(fun kvp ->
            key kvp,value kvp
        )


