module FSharp.Idioms.Literals.ValuePrinterUtils

open System
open FSharp.Idioms.Literals.ValuePrinterImpls

let printers = [
    boolValuePrinter
    stringValuePrinter
    charValuePrinter
    sbyteValuePrinter
    byteValuePrinter
    int16ValuePrinter
    uint16ValuePrinter
    intValuePrinter
    uint32ValuePrinter
    int64ValuePrinter
    uint64ValuePrinter
    singleValuePrinter
    floatValuePrinter
    decimalValuePrinter
    nativeintValuePrinter
    unativeintValuePrinter
    bigintValuePrinter
    unitValuePrinter
    DBNullValuePrinter
    GuidValuePrinter
    enumValuePrinter
    timeSpanValuePrinter
    dateTimeOffsetValuePrinter
    dateTimeValuePrinter
    nullableValuePrinter
    arrayValuePrinter
    listValuePrinter
    setValuePrinter
    hashsetValuePrinter
    mapValuePrinter
    tupleValuePrinter
    unionValuePrinter
    recordValuePrinter
    TypeValuePrinter
    //nullValuePrinter
    //underlyingValuePrinter
    ]

/// 根据优先级确定表达式是否带括号
let rec valueStringify (printers:list<Type->ValuePrinter>) (ty:Type) =
    let pickedStringify =
        printers
        |> Seq.tryPick(fun getPrinter -> 
            let printer = getPrinter ty
            if printer.finder then
                Some printer.print
            else None)
        |> Option.defaultValue(fun loop value precContext ->
            ////没有类型信息，null,nullable,None都打印成null
            if ty = null || ty = typeof<obj> then
                match value with
                | null -> "null"
                | _ ->
                    let underlyingType = value.GetType()
                    if underlyingType <> typeof<obj> then
                        loop underlyingType value precContext
                    else sprintf "%A" value
            else failwith $"{ty}"
            )

    pickedStringify (valueStringify printers)



