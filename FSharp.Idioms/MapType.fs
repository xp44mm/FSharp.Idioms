module FSharp.Idioms.MapType

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection

open Microsoft.FSharp.Reflection

let mapModuleType = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.MapModule")
let mToArrayDef = mapModuleType.GetMethod "ToArray"
let mOfArrayDef = mapModuleType.GetMethod "OfArray"

let memoElementType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
let getElementType (mapType:Type) =
    let valueFactory (ty:Type) = 
        FSharpType.MakeTupleType(ty.GenericTypeArguments)
    memoElementType.GetOrAdd(mapType, valueFactory)

let memoMakeArrayType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
let makeArrayType (mapType:Type) =
    let valueFactory (mapType:Type) =
        let elementType = getElementType mapType
        elementType.MakeArrayType()
    memoMakeArrayType.GetOrAdd(mapType,valueFactory)

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


let memoReadMap = ConcurrentDictionary<Type, obj -> Type*obj[]>(HashIdentity.Structural)

///Map转化为数组
[<Obsolete("MapType.toArray")>]
let readMap (mapType:Type) =
    let valueFactory (ty:Type) =
        let arrayType = makeArrayType ty
        let readArray = ArrayType.toArray // arrayType
        let mToArray = getToArray ty        
        fun (mp:obj) ->
            let arr =
                mToArray.Invoke(null,[|mp|])
                |> readArray
            arr.GetType().GetElementType(),arr
    memoReadMap.GetOrAdd(mapType,Func<_,_>valueFactory)

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


