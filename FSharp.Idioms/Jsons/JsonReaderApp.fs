module FSharp.Idioms.Jsons.JsonReaderApp

open FSharp.Idioms
open FSharp.Reflection

open System
open System.Collections.Generic
open FSharp.Idioms.Jsons.JsonReaders

let readers = [
    tryBool
    tryString
    tryChar
    trySbyte
    tryByte
    tryInt16
    tryUint16
    tryInt
    tryUint32
    tryInt64
    tryUint64
    trySingle
    tryFloat
    tryDecimal
    tryNativeint
    tryUnativeint
    tryUnit
    tryDBNull
    tryOption
    tryNullable
    tryEnum
    tryArray
    tryIEnumerable
    tryTuple
    tryUnion
    tryRecord
    tryClass

    ]

/// 根据优先级确定表达式是否带括号
let rec mainRead (readers:list<Type->option<Loop->obj->Json>>) (ty:Type) =
    let pickedFn =
        readers
        |> Seq.tryPick(fun fn -> fn ty )
        |> Option.defaultValue(fun loop value ->
            if ty = null || ty = typeof<obj> then
                match value with
                | null -> Json.Null
                | _ ->
                    let vty = value.GetType()
                    if vty <> typeof<obj> then
                        loop vty value
                    else failwithf "没有类型信息，且%A" value
            else failwith $"未实现的类型{ty}"
            )

    pickedFn (mainRead readers)

let readDynamic : Type -> obj -> Json = mainRead readers
