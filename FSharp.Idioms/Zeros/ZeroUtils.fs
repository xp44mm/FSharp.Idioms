module FSharp.Idioms.Zeros.ZeroUtils

open System
open FSharp.Idioms.Zeros.TryZeros
open FSharp.Idioms.Literals

let tries = [
    tryBool
    trySbyte
    tryInt16
    tryInt
    tryInt64
    tryNativeint
    tryByte
    tryUint16
    tryChar
    tryUint32
    tryUint64
    tryUnativeint
    tryDecimal
    tryFloat
    tryFloat32
    tryBigint
    tryString
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
    tryValueType
    tryCtor
    ]

/// 主函数
let rec getZero (tries: list<Type->option<Loop->obj>>) (ty: Type) =
    let action =
        tries
        |> Seq.tryPick (fun g -> g ty)
        |> Option.defaultValue (fun _ -> 
            let outp = TypePrinterApp.typeStringify TypePrinterApp.typePrinters ty 0
            failwith $"未实现类型:{outp}")
    action (getZero tries)
