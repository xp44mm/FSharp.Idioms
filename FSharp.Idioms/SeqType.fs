module FSharp.Idioms.SeqType

open System
open System.Collections
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection

let seqTypeDef = typeof<seq<_>>.GetGenericTypeDefinition()
let giterorDef = typeof<IEnumerator<_>>.GetGenericTypeDefinition()
let biteror = typeof<IEnumerator>
let mMoveNext = biteror.GetMethod("MoveNext")

/// 读取序列中的元素
let seqReader =
    let dic = ConcurrentDictionary<Type, obj -> obj[]>(HashIdentity.Structural)
    fun (ty:Type) ->
        if dic.ContainsKey(ty) |> not then
            let elemType = ty.GenericTypeArguments.[0]
            let seqType = seqTypeDef.MakeGenericType(elemType)
            let mGetEnumerator = seqType.GetMethod("GetEnumerator")
            let giteror = giterorDef.MakeGenericType(elemType)
            let pCurrent = giteror.GetProperty("Current")

            let reader ls =
                let enumerator = mGetEnumerator.Invoke(ls,[||])
                [|
                    while(mMoveNext.Invoke(enumerator,[||])|>unbox<bool>) do
                        yield pCurrent.GetValue(enumerator)
                |]
            dic.TryAdd(ty, reader) |> ignore
        dic.[ty]


