module FSharp.Idioms.Literals.TypePrinterApp

open System
open FSharp.Idioms
open FSharp.Idioms.Literals.TypePrinters
open System.Collections.Concurrent

let typePrinters = [
    tryEnum
    tryUnit
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
    tryNativeint
    tryUnativeint
    tryDecimal
    tryBigint
    tryArray
    tryTuple
    tryAnonymousRecord
    tryFunction
    tryGenericTypeDefinition
    tryGenericType
    ]

/// 根据优先级确定表达式是否带括号
let rec typeStringify (printers:list<Type->option<(Type->int->string)->int->string>>) =
    let memo = ConcurrentDictionary<Type, int -> string>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let picked =
            printers
            |> Seq.tryPick(fun tryPrint -> tryPrint ty)
            |> Option.defaultValue(fun loop prec -> ty.Name)

        picked (typeStringify printers)
        )

