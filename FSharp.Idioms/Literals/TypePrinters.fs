module FSharp.Idioms.Literals.TypePrinters

open System
open System.Text.RegularExpressions
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Literals.Paren

type Loop = Type->int->string

let tryEnum =
    fun (ty:Type) ->
        if ty.IsEnum then
            Some(fun loop prec -> ty.Name)
        else None

let tryUnit =
    fun (ty:Type) ->
        if ty = typeof<unit> then
            Some(fun loop prec -> "unit")
        else None

let tryBool =
    fun (ty:Type) ->
        if ty = typeof<bool> then
            Some(fun loop prec -> "bool")
        else None

let tryString =
    fun (ty:Type) ->
        if ty = typeof<string> then
            Some(fun loop prec -> "string")
        else None


let tryChar =
    fun (ty:Type) ->
        if ty = typeof<char> then
            Some(fun loop prec -> "char")
        else None


let trySbyte =
    fun (ty:Type) ->
        if ty = typeof<sbyte> then
            Some(fun loop prec -> "sbyte")
        else None


let tryByte =
    fun (ty:Type) ->
        if ty = typeof<byte> then
            Some(fun loop prec -> "byte")
        else None


let tryInt16 =
    fun (ty:Type) ->
        if ty = typeof<int16> then
            Some(fun loop prec -> "int16")
        else None


let tryUint16 =
    fun (ty:Type) ->
        if ty = typeof<uint16> then
            Some(fun loop prec -> "uint16")
        else None


let tryInt =
    fun (ty:Type) ->
        if ty = typeof<int> then
            Some(fun loop prec -> "int")
        else None

let tryUint32 =
    fun (ty:Type) ->
        if ty = typeof<uint32> then
            Some(fun loop prec -> "uint32")
        else None


let tryInt64 =
    fun (ty:Type) ->
        if ty = typeof<int64> then
            Some(fun loop prec -> "int64")
        else None

let tryUint64 =
    fun (ty:Type) ->
        if ty = typeof<uint64> then
            Some(fun loop prec -> "uint64")
        else None

let trySingle =
    fun (ty:Type) ->
        if ty = typeof<single> then
            Some(fun loop prec -> "single")
        else None

let tryFloat =
    fun (ty:Type) ->
        if ty = typeof<float> then
            Some(fun loop prec -> "float")
        else None

let tryNativeint =
    fun (ty:Type) ->
        if ty = typeof<nativeint> then
            Some(fun loop prec -> "nativeint")
        else None

let tryUnativeint =
    fun (ty:Type) ->
        if ty = typeof<unativeint> then
            Some(fun loop prec -> "unativeint")
        else None

let tryDecimal =
    fun (ty:Type) ->
        if ty = typeof<decimal> then
            Some(fun loop prec -> "decimal")
        else None

let tryBigint =
    fun (ty:Type) ->
        if ty = typeof<bigint> then
            Some(fun loop prec -> "bigint")
        else None

let tryArray =
    fun (ty:Type) ->
        if ty.IsArray && ty.GetArrayRank() = 1 then
            Some(fun loop prec ->
                let arrayPrec = typePrecedences.["[]"]
                let elemType = ty.GetElementType()
                loop elemType arrayPrec + "[]"
                |> putParen prec arrayPrec)
        else None

let tryTuple =
    fun (ty:Type) ->
        if FSharpType.IsTuple ty then
            Some(fun loop prec ->
                let tuplePrec = typePrecedences.["*"]
                ty
                |> FSharpType.GetTupleElements
                |> Array.map(fun ety -> loop ety (tuplePrec+1))
                |> String.concat "*"
                |> putParen prec tuplePrec)
        else None

let tryAnonymousRecord =
    fun (ty:Type) ->
        if 
            FSharpType.IsRecord ty && 
            Regex.IsMatch(ty.Name,"^<>f__AnonymousType\d+`\d+$") 
        then
            Some(fun loop prec ->
                FSharpType.GetRecordFields ty
                |> Array.map(fun pi -> 
                    loop pi.PropertyType typePrecedences.[":"]
                    |> sprintf "%s:%s" pi.Name)
                |> String.concat ";"
                |> sprintf "{|%s|}"
            )
        else None

let tryFunction =
    fun (ty:Type) ->
        if FSharpType.IsFunction ty then
            Some(fun loop prec ->
                let domainType,rangeType = FSharpType.GetFunctionElements ty
                let funPrec = typePrecedences.["->"]
                let domainType = loop domainType (funPrec+1)
                let rangeType  = loop rangeType (funPrec-1)
                sprintf "%s->%s" domainType rangeType
                |> putParen prec funPrec
                )
        else None

let tryGenericTypeDefinition =
    fun (ty:Type) ->
        if ty.IsGenericType && ty.IsGenericTypeDefinition then
            Some(fun loop prec ->
                let name = FSharpCodeUtils.getGenericTypeName ty.Name

                ty.GetGenericArguments()
                |> Array.filter(fun p -> p.IsGenericParameter)
                |> Array.sortBy(fun p -> p.GenericParameterPosition)
                |> Array.map(fun t -> "'" + t.Name)
                |> String.concat ","
                |> sprintf "%s<%s>" name
                |> putParen prec typePrecedences.["<>"]
                )
        else None

let tryGenericType =
    fun (ty:Type) ->
        if ty.IsGenericType then
            Some(fun loop prec ->
                let name = FSharpCodeUtils.getGenericTypeName ty.Name

                ty.GenericTypeArguments
                |> Array.map(fun aty -> loop aty typePrecedences.[","])
                |> String.concat ","
                |> sprintf "%s<%s>" name
                |> putParen prec typePrecedences.["<>"]
                )
        else None


