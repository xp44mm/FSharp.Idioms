module FSharp.Idioms.Literals.ValuePrinterImpls

open System
open System.Globalization
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Literals.Paren


let boolValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<bool>
    print = fun loop ->
        let b = unbox<bool> value
        if b then "true" else "false"
    }

let stringValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<string>
    print = fun loop ->
        unbox<string> value
        |> FSharpCodeUtils.toStringLiteral
    }

let charValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<char>
    print = fun loop ->
        unbox<char> value
        |> FSharpCodeUtils.toCharLiteral
    }

let sbyteValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<sbyte>
    print = fun loop ->
        let value = unbox<sbyte> value
        Convert.ToString value + "y"
    }

let byteValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<byte>
    print = fun loop ->
        let value = unbox<byte> value
        Convert.ToString value + "uy"
    }

let int16ValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<int16>
    print = fun loop ->
        let value = unbox<int16> value
        Convert.ToString value + "s"
    }

let uint16ValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<uint16>
    print = fun loop ->
        let value = unbox<uint16> value
        Convert.ToString value + "us"
    }

let intValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<int>
    print = fun loop ->
        let value = unbox<int> value
        Convert.ToString value
    }

let uint32ValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<int>
    print = fun loop ->
        let value = unbox<uint32> value
        Convert.ToString value  + "u"
    }

let int64ValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<int64>
    print = fun loop ->
        let value = unbox<int64> value
        Convert.ToString value  + "L"
    }

let uint64ValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<uint64>
    print = fun loop ->
        let value = unbox<uint64> value
        Convert.ToString value + "UL"
    }

let singleValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<single>
    print = fun loop ->
        let value = unbox<single> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G9"
        s + "f"
    }

let floatValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<float>
    print = fun loop ->
        let value = unbox<float> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G17"
        FSharpCodeUtils.decimalPoint s
    }

let decimalValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<decimal>
    print = fun loop ->
        let value = unbox<decimal> value
        Convert.ToString value + "M"
    }

let nativeintValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<nativeint>
    print = fun loop ->
        let value = unbox<nativeint> value
        Convert.ToString value + "n"
    }

let unativeintValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<unativeint>
    print = fun loop ->
        let value = unbox<unativeint> value
        Convert.ToString value + "un"
    }
    
let bigintValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<bigint>
    print = fun loop ->
        let value = unbox<bigint> value
        Convert.ToString value + "I"
    }

let unitValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<unit>
    print = fun loop -> "()"
    }

let DBNullValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<DBNull> || DBNull.Value.Equals value
    print = fun loop ->
        "DBNull.Value"
    }

let GuidValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<Guid>
    print = fun loop ->
        let id = unbox<Guid> value
        sprintf "Guid(\"%s\")" <| id.ToString()
        |> putParen precContext valuePrecedences.[" "]
    }

let enumValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty.IsEnum
    print = fun loop ->
        if ty.IsDefined(typeof<FlagsAttribute>,false) then
            let reader = EnumType.readFlags ty
            reader value
            |> Array.map(fun enm -> sprintf "%s.%s" ty.Name enm )
            |> String.concat "|||"
            |> putParen precContext valuePrecedences.["|||"]
        else
            Enum.GetName(ty,value)
            |> sprintf "%s.%s" ty.Name
            |> putParen precContext valuePrecedences.["."]
    }

let timeSpanValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<TimeSpan>
    print = fun loop ->
        let v = unbox<TimeSpan> value
        [
            v.Days
            v.Hours
            v.Minutes
            v.Seconds
            v.Milliseconds
        ]
        |> List.map(fun i -> Convert.ToString i)
        |> String.concat ","
        |> sprintf "TimeSpan(%s)"
        |> putParen precContext valuePrecedences.[" "]
    }

let dateTimeOffsetValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<DateTimeOffset>
    print = fun loop ->
        let thisDate = unbox<DateTimeOffset> value
        [
            thisDate.Year       .ToString()
            thisDate.Month      .ToString()
            thisDate.Day        .ToString()
            thisDate.Hour       .ToString()
            thisDate.Minute     .ToString()
            thisDate.Second     .ToString()
            thisDate.Millisecond.ToString()
            thisDate.Offset |> loop valuePrecedences.[","] typeof<TimeSpan>
        ]
        |> String.concat ","
        |> sprintf "DateTimeOffset(%s)"
        |> putParen precContext valuePrecedences.[" "]
    }

let dateTimeValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = ty = typeof<DateTime>
    print = fun loop ->
        let dt = unbox<DateTime> value
        loop precContext typeof<DateTimeOffset> (DateTimeOffset dt)
    }
    
let nullableValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = 
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    print = fun loop ->
        if value = null then
            "Nullable()"
        else
            let underlyingType = ty.GenericTypeArguments.[0]
            loop valuePrecedences.[" "] underlyingType value
            |> sprintf "Nullable %s"
        |> putParen precContext valuePrecedences.[" "]
    }

let arrayValuePrinter (prec:int) (ty:Type) (value:obj) =
    {
    finder =
        ty.IsArray && ty.GetArrayRank() = 1

    print = fun (loop:int -> Type -> obj -> string) ->
        let reader = ArrayType.readArray ty
        let elemType, elements = reader value

        elements
        |> Array.map(loop valuePrecedences.[";"] elemType)
        |> String.concat ";"
        |> sprintf "[|%s|]"  //一定無需加括號
    
    }

let listValuePrinter (prec:int) (ty:Type) (value:obj) =
    { 
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<list<_>>
    print = fun loop ->
        let reader = ListType.readList ty
        let elemType, elements = reader value

        elements
        |> Array.map(loop valuePrecedences.[";"] elemType)
        |> String.concat ";"
        |> sprintf "[%s]" //一定無需加括號
    }

let setValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
    print = fun (loop:int -> Type -> obj -> string) ->
        let reader = SetType.readSet ty
        let elemType, elements = reader value

        elements
        |> Array.map(loop valuePrecedences.[";"] elemType)
        |> String.concat ";"
        |> sprintf "set [%s]"
        |> putParen precContext valuePrecedences.[" "]
    }

let hashsetValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<System.Collections.Generic.HashSet<_>>
    print = fun (loop:int -> Type -> obj -> string) ->
        let reader = SeqType.seqReader ty
        let elements = reader value
        let elemType = ty.GenericTypeArguments.[0]

        elements
        |> Array.map(loop valuePrecedences.[";"] elemType)
        |> String.concat ";"
        |> sprintf "HashSet [%s]"
        |> putParen precContext valuePrecedences.[" "]

    }

let mapValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>>
    print = fun (loop:int -> Type -> obj -> string) ->
        let reader = MapType.readMap ty
        let elemType, elements = reader value

        elements
        |> Array.map(loop valuePrecedences.[";"] elemType)
        |> String.concat ";"
        |> sprintf "Map [%s]"
        |> putParen precContext valuePrecedences.[" "]
    }

let tupleValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
        finder = FSharpType.IsTuple ty
        print = fun loop ->
            let reader = TupleType.readTuple ty
            let fields = reader value

            fields
            |> Array.map(fun(ftype,field)-> loop valuePrecedences.[","] ftype field)
            |> String.concat ","
            |> putParen precContext valuePrecedences.[","]
    }

let unionValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
        finder = FSharpType.IsUnion ty
        print = fun loop ->
            let reader = UnionType.readUnion ty
            let name,fields = reader value
            let qa = UnionType.getQualifiedAccess ty
            let name = qa + name

            match fields with
            | [||] -> name
            | [|ftype,field|] ->
                let payload = loop valuePrecedences.[" "] ftype field
                if payload.StartsWith("(") then name + payload else name + " " + payload
                |> putParen precContext valuePrecedences.[" "]
            | _ ->
                fields
                |> Array.map(fun(ftype,field)-> loop valuePrecedences.[","] ftype field)
                |> String.concat ","
                |> sprintf "%s(%s)" name
                |> putParen precContext valuePrecedences.[" "]
    }

let recordValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
        finder = FSharpType.IsRecord ty
        print = fun loop ->
            let reader = RecordType.readRecord ty
            let fields = reader value

            fields
            |> Array.map(fun(pi,value)->
                let nm = 
                    if FSharpCodeUtils.isIdentifier pi.Name then
                        pi.Name 
                    else String.Format("``{0}``",pi.Name)

                let value = loop 0 pi.PropertyType value
                String.Format("{0}= {1}",nm,value)
            )
            |> String.concat ";"
            |> sprintf "{%s}"
    }

//let typeValuePrinter (precContext:int) (ty:Type) (value:obj) =
//    {
//        finder = ty = typeof<Type>
//        print = fun loop ->
//            TypePrinterUtils.stringifyTypeDynamic (unbox<Type> value)
//            |> sprintf "typeof<%s>"
//    }

//没有类型信息，null,nullable,None都打印成null
let nullValuePrinter (precContext:int) (ty:Type) (value:obj) =
    {
    finder = value = null
    print = fun loop -> "null"
    }

let underlyingValuePrinter (precContext:int) (ty:Type) (value:obj) =
    let underlyingType = value.GetType()
    {
    finder =
        // underlyingType.IsSubclassOf(ty)
        ty = typeof<obj> && value.GetType() <> typeof<obj>
    print = fun loop ->
        loop precContext underlyingType value
    }

