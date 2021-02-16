module FSharp.Idioms.TupleType

open System
open Microsoft.FSharp.Reflection
open System.Collections.Concurrent

let memoPreComputeTupleReader = ConcurrentDictionary<Type, obj -> obj[]>(HashIdentity.Structural)
///元组分解一次为元素
let preComputeTupleReader (tupleType:Type) =
    memoPreComputeTupleReader.GetOrAdd(tupleType,Func<_,_> FSharpValue.PreComputeTupleReader)

let memoTupleElementTypes = ConcurrentDictionary<Type, Type[]>(HashIdentity.Structural)
let getTupleElementTypes (tupleType:Type) =
    memoTupleElementTypes.GetOrAdd(tupleType,FSharpType.GetTupleElements)

let memoReadTuple = ConcurrentDictionary<Type, obj -> (Type*obj)[]>(HashIdentity.Structural)
let readTuple (tupleType:Type) =
    let valueFactory (ty:Type) =
        let reader = preComputeTupleReader ty
        let elementTypes = getTupleElementTypes(ty)
        fun (tuple:obj) ->
            let elements = reader tuple
            Array.zip elementTypes elements
    memoReadTuple.GetOrAdd(tupleType,Func<_,_> valueFactory)

