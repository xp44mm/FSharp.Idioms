module FSharp.Idioms.SetType

open System
open System.Collections.Concurrent
open System.Reflection

let setModuleType  = FSharpModules.fsharpAssembly.GetType("Microsoft.FSharp.Collections.SetModule")

let mToArrayDef = setModuleType.GetMethod "ToArray"
let mOfArrayDef = setModuleType.GetMethod "OfArray"

//let memoElementType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
//let getElementType (setType:Type) =
//    let valueFactory (ty:Type) = ty.GenericTypeArguments.[0]
//    memoElementType.GetOrAdd(setType, valueFactory)

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

let memoMakeArrayType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)

let makeArrayType (setType:Type) =
    let valueFactory (ty:Type) =
        let elementType = ty.GenericTypeArguments.[0]
        elementType.MakeArrayType()
    memoMakeArrayType.GetOrAdd(setType,valueFactory)

let memoReadSet = ConcurrentDictionary<Type, obj -> Type*obj[]>(HashIdentity.Structural)

///Set可以讀取到集合中的元素
[<Obsolete("IEnumerableType.toArray")>]
let readSet (setType:Type) =
    let valueFactory (ty:Type) =
        let mToArray = getToArray ty
        let elemType = setType.GenericTypeArguments.[0]
        let read = ArrayType.toArray
            
        fun (st:obj) ->
            let arr =
                mToArray.Invoke(null,[|st|])
                |> read
            elemType,arr
    memoReadSet.GetOrAdd(setType,Func<_,_> valueFactory)

