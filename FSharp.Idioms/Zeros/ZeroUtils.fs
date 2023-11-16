module FSharp.Idioms.Zeros.ZeroUtils

open System
open System.Reflection
open FSharp.Idioms.Zeros.TryZeros

let tries = [
    tryBool
    trysbyte
    tryint16
    tryint
    tryint64
    trynativeint
    trybyte
    tryuint16
    trychar
    tryuint32
    tryuint64
    tryunativeint
    trydecimal
    tryfloat
    tryfloat32
    trybigint
    trystring
    tryDBNull
    tryDateTimeOffset
    tryTimeSpan
    tryEnum
    tryNullable
    tryOption
    tryArray
    tryList
    trySet
    tryMap
    tryTuple
    tryRecord
    tryUnion
    tryZero
    ]

/// 主函数
let rec getZero (tries: list<Type->option<Loop->obj>>) (ty: Type) =
    let action =
        tries
        |> Seq.tryPick (fun g -> g ty)
        |> Option.defaultValue (fun _ -> failwith $"{ty}")
    action (getZero tries)
