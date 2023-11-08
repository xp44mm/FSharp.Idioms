module FSharp.Idioms.Literals.TypePrinterImpls

open System
open System.Text.RegularExpressions
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Literals.Paren

let enumTypePrinter (ty:Type) = 
    {
    finder = ty.IsEnum
    print = fun loop prec -> ty.Name
    }
let unitTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<unit>
    print = fun loop prec -> "unit"
    }
let boolTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<bool>
    print = fun loop prec -> "bool"
    }
let stringTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<string>
    print = fun loop prec -> "string"
    }
let charTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<char>
    print = fun loop prec -> "char"
    }
let sbyteTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<sbyte>
    print = fun loop prec -> "sbyte"
    }
let byteTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<byte>
    print = fun loop prec -> "byte"
    }
let int16TypePrinter (ty:Type) = 
    {
    finder = ty = typeof<int16>
    print = fun loop prec -> "int16"
    }
let uint16TypePrinter (ty:Type) = 
    {
    finder = ty = typeof<uint16>
    print = fun loop prec -> "uint16"
    }
let intTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<int>
    print = fun loop prec -> "int"
    }
let uint32TypePrinter (ty:Type) = 
    {
    finder = ty = typeof<uint32>
    print = fun loop prec -> "uint32"
    }
let int64TypePrinter (ty:Type) = 
    {
    finder = ty = typeof<int64>
    print = fun loop prec -> "int64"
    }
let uint64TypePrinter (ty:Type) = 
    {
    finder = ty = typeof<uint64>
    print = fun loop prec -> "uint64"
    }
let singleTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<single>
    print = fun loop prec -> "single"
    }
let floatTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<float>
    print = fun loop prec -> "float"
    }
let decimalTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<decimal>
    print = fun loop prec -> "decimal"
    }
let nativeintTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<nativeint>
    print = fun loop prec -> "nativeint"
    }
let unativeintTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<unativeint>
    print = fun loop prec -> "unativeint"
    }
let bigintTypePrinter (ty:Type) = 
    {
    finder = ty = typeof<bigint>
    print = fun loop prec -> "bigint"
    }
let arrayTypePrinter (ty:Type) = 
    {
    finder = ty.IsArray && ty.GetArrayRank() = 1
    print = fun loop prec ->
        let arrayPrec = typePrecedences.["[]"]
        let elemType = ty.GetElementType()
        loop elemType arrayPrec + "[]"
        |> putParen prec arrayPrec
    }

let tupleTypePrinter (ty:Type) = 
    {
    finder = FSharpType.IsTuple ty
    print = fun loop prec ->
        let tuplePrec = typePrecedences.["*"]
        ty
        |> FSharpType.GetTupleElements
        |> Array.map(fun ety -> loop ety (tuplePrec+1))
        |> String.concat "*"
        |> putParen prec tuplePrec
    }
let AnonymousRecordTypePrinter (ty:Type) = 
    {
    finder = 
        FSharpType.IsRecord ty && 
        Regex.IsMatch(ty.Name,"^<>f__AnonymousType\d+`\d+$")
    print = fun loop prec ->
        FSharpType.GetRecordFields ty
        |> Array.map(fun pi -> 
            loop pi.PropertyType typePrecedences.[":"]
            |> sprintf "%s:%s" pi.Name)
        |> String.concat ";"
        |> sprintf "{|%s|}"
    }

let FunctionTypePrinter (ty:Type) = 
    {
    finder = FSharpType.IsFunction ty
    print = fun loop prec ->
        let domainType,rangeType = FSharpType.GetFunctionElements ty
        let funPrec = typePrecedences.["->"]
        let domainType = loop domainType (funPrec+1)
        let rangeType  = loop rangeType (funPrec-1)
        sprintf "%s->%s" domainType rangeType
        |> putParen prec funPrec
    }


let GenericTypeDefinitionTypePrinter (ty:Type) = 
    {
    finder = ty.IsGenericType && ty.IsGenericTypeDefinition
    print = fun loop prec ->
        let name = FSharpCodeUtils.getGenericTypeName ty.Name

        ty.GetGenericArguments()
        |> Array.filter(fun p -> p.IsGenericParameter)
        |> Array.sortBy(fun p -> p.GenericParameterPosition)
        |> Array.map(fun t -> "'" + t.Name)
        |> String.concat ","
        |> sprintf "%s<%s>" name
        |> putParen prec typePrecedences.["<>"]
    }

let GenericTypePrinter (ty:Type) = 
    {
    finder = ty.IsGenericType
    print = fun loop prec ->
        let name = FSharpCodeUtils.getGenericTypeName ty.Name

        ty.GenericTypeArguments
        |> Array.map(fun aty -> loop aty typePrecedences.[","])
        |> String.concat ","
        |> sprintf "%s<%s>" name
        |> putParen prec typePrecedences.["<>"]
    }

