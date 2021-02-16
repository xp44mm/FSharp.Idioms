module FSharp.Idioms.MapType

open System
open Microsoft.FSharp.Reflection
open System.Collections.Concurrent
open System.Reflection

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
let readMap (mapType:Type) =
    let valueFactory (ty:Type) =
        let arrayType = makeArrayType ty
        let readArray = ArrayType.readArray arrayType
        let mToArray = getToArray ty

        fun (mp:obj) ->
            mToArray.Invoke(null,[|mp|])
            |> readArray
    memoReadMap.GetOrAdd(mapType,Func<_,_>valueFactory)


