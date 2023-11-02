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
    fun prec ty value -> {
    finder = ty = typeof<Type>
    print = fun loop ->
        value
        |> unbox<Type>
        |> TypePrinterUtils.typeStringify TypePrinterUtils.printers 0
        |> sprintf "typeof<%s>"
    }
    nullValuePrinter
    underlyingValuePrinter
    ]

/// 根据优先级确定表达式是否带括号
let rec valueStringify (printers:list<int->Type->obj->ValuePrinter>) (prec:int) (ty:Type) (value:obj) =
    let pickedStringify =
        printers
        |> Seq.tryPick(fun getPrinter -> 
            let printer = getPrinter prec ty value
            if printer.finder then
                Some printer.print
            else None)
        |> Option.defaultValue(fun loop -> sprintf "%A" value)

    pickedStringify (valueStringify printers)


