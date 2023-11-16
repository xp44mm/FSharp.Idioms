module FSharp.Idioms.Literals.ValuePrinterApp

open System
open FSharp.Idioms.Literals.ValuePrinters

let printers = [
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
    tryBigint
    tryUnit
    tryDBNull
    tryGuid
    tryEnum
    tryTimeSpan
    tryDateTimeOffset
    tryDateTime
    tryNullable
    tryArray
    tryList
    trySet
    tryHashSet
    tryMap
    tryTuple
    tryUnion
    tryRecord
    tryType
    ]

/// 根据优先级确定表达式是否带括号
let rec valueStringify (printers:list<Type->option<Loop->obj->int->string>>) (ty:Type) =
    let pickedStringify =
        printers
        |> Seq.tryPick(fun getPrinter -> getPrinter ty )
        |> Option.defaultValue(fun loop value precContext ->
            ////没有类型信息，null,nullable,None都打印成null
            if ty = null || ty = typeof<obj> then
                match value with
                | null -> "null"
                | _ ->
                    let underlyingType = value.GetType()
                    if underlyingType <> typeof<obj> then
                        loop underlyingType value precContext
                    else failwithf "%A" value
            else failwith $"{ty}"
            )

    pickedStringify (valueStringify printers)




