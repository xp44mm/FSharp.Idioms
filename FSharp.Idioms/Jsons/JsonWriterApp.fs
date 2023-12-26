module FSharp.Idioms.Jsons.JsonWriterApp // 'obj -> Json

open FSharp.Idioms
open FSharp.Idioms.Jsons.JsonWriters
open FSharp.Reflection

open System
open System.Collections.Generic

let writers = [
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
    tryTuple
    tryRecord
    tryList
    trySet
    tryHashSet
    tryMap
    tryUnion
    ]

/// 根据优先级确定表达式是否带括号
let rec mainWrite (writers:list<Type->option<Loop->Json->obj>>) (ty:Type) =
    let pickedFn =
        writers
        |> Seq.tryPick(fun fn -> fn ty )
        |> Option.defaultValue(fun loop json ->
            if ty = null || ty = typeof<obj> then
                match json with
                | Json.Object fields -> failwith $"{json}"
                | Json.Array elems -> failwith $"{json}"
                | Json.Null-> null
                | Json.False -> box false
                | Json.True -> box true
                | Json.String s -> box s
                | Json.Number x -> box x
                | Json.Decimal x -> box x
            else failwith $"未实现的类型:{ty}"
            )

    pickedFn (mainWrite writers)

let writeDynamic : Type->Json->obj  = mainWrite writers
