module FSharp.Idioms.ArrayType

open System
open System.Collections.Concurrent
open System.Reflection

let memoElementType = ConcurrentDictionary<Type, Type>(HashIdentity.Structural)
let getElementType (arrayType:Type) =
    let valueFactory (ty:Type) = ty.GetElementType()
    memoElementType.GetOrAdd(arrayType, valueFactory)

let memoLengthProperties = ConcurrentDictionary<Type, PropertyInfo>(HashIdentity.Structural)
let getLength (arrayType:Type) =   
    let valueFactory (arrayType:Type) = arrayType.GetProperty("Length")
    let pLength = memoLengthProperties.GetOrAdd(arrayType, valueFactory)
    fun (arr:obj) -> pLength.GetValue(arr) :?> int

let memoGetValueMethods = ConcurrentDictionary<Type, MethodInfo>(HashIdentity.Structural)
let invokeGetValue (arrayType:Type) =
    let valueFactory (arrayType:Type) = arrayType.GetMethod("GetValue",Array.singleton(typeof<int>))
    let mGetValue = memoGetValueMethods.GetOrAdd(arrayType,valueFactory)
    fun (arr:obj,i:int) -> mGetValue.Invoke(arr,Array.singleton(box i))

let memoReadArray = ConcurrentDictionary<Type, obj -> Type*obj[]>(HashIdentity.Structural)

///数组分解一次为元素
let readArray (arrayType:Type) =
    let valueFactory (arrayType:Type) =
        let getLength = getLength arrayType
        let invokeGetValue = invokeGetValue arrayType
        let elemType = getElementType arrayType

        fun (arr:obj) ->
            let len = getLength arr
            let elements =
                Array.create len 0
                |> Array.mapi(fun i _ -> invokeGetValue (arr,i))
            elemType,elements

    memoReadArray.GetOrAdd(arrayType, Func<_,_> valueFactory)

