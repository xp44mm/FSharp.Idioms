module FSharp.Idioms.SeqType

open System
open System.Collections
open System.Collections.Concurrent
open System.Collections.Generic

let seqTypeDef = typedefof<seq<_>>
let giterorDef = typedefof<IEnumerator<_>>
let biteror = typeof<IEnumerator>
let mMoveNext = biteror.GetMethod("MoveNext")

/// 读取序列中的元素
let seqReader =
    let dic = ConcurrentDictionary<Type, obj -> obj[]>(HashIdentity.Structural)
    fun (ty:Type) ->
        if dic.ContainsKey(ty) |> not then
            let elemType = ty.GenericTypeArguments.[0]
            let seqType = seqTypeDef.MakeGenericType(elemType)
            let giteror = giterorDef.MakeGenericType(elemType)
            let mGetEnumerator = seqType.GetMethod("GetEnumerator")
            let pCurrent = giteror.GetProperty("Current")

            let reader ls =
                let enumerator = mGetEnumerator.Invoke(ls,[||])
                [|
                    while(mMoveNext.Invoke(enumerator,[||])|>unbox<bool>) do
                        yield pCurrent.GetValue(enumerator)
                |]
            dic.TryAdd(ty, reader) |> ignore
        dic.[ty]


