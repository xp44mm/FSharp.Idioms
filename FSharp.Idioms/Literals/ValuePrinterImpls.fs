module FSharp.Idioms.Literals.ValuePrinterImpls

open System
open System.Globalization
open FSharp.Reflection
open FSharp.Idioms
open FSharp.Idioms.Literals.Paren


let boolValuePrinter (ty:Type) =
    {
    finder = ty = typeof<bool>
    print = fun loop (value:obj) (precContext:int) ->
        let b = unbox<bool> value
        if b then "true" else "false"
    }

let stringValuePrinter (ty:Type) =
    {
    finder = ty = typeof<string>
    print = fun loop (value:obj) (precContext:int) ->
        unbox<string> value
        |> FSharpCodeUtils.toStringLiteral
    }

let charValuePrinter (ty:Type) =
    {
    finder = ty = typeof<char>
    print = fun loop (value:obj) (precContext:int) ->
        unbox<char> value
        |> FSharpCodeUtils.toCharLiteral
    }

let sbyteValuePrinter (ty:Type) =
    {
    finder = ty = typeof<sbyte>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<sbyte> value
        Convert.ToString value + "y"
    }

let byteValuePrinter (ty:Type) =
    {
    finder = ty = typeof<byte>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<byte> value
        Convert.ToString value + "uy"
    }

let int16ValuePrinter (ty:Type) =
    {
    finder = ty = typeof<int16>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<int16> value
        Convert.ToString value + "s"
    }

let uint16ValuePrinter (ty:Type) =
    {
    finder = ty = typeof<uint16>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<uint16> value
        Convert.ToString value + "us"
    }

let intValuePrinter (ty:Type) =
    {
    finder = ty = typeof<int>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<int> value
        Convert.ToString value
    }

let uint32ValuePrinter (ty:Type) =
    {
    finder = ty = typeof<uint32>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<uint32> value
        Convert.ToString value  + "u"
    }

let int64ValuePrinter (ty:Type) =
    {
    finder = ty = typeof<int64>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<int64> value
        Convert.ToString value  + "L"
    }

let uint64ValuePrinter (ty:Type) =
    {
    finder = ty = typeof<uint64>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<uint64> value
        Convert.ToString value + "UL"
    }

let singleValuePrinter (ty:Type) =
    {
    finder = ty = typeof<single>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<single> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G9"
        s + "f"
    }

let floatValuePrinter (ty:Type) =
    {
    finder = ty = typeof<float>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<float> value
        let s = value.ToString("R", CultureInfo.InvariantCulture) // "G17"
        FSharpCodeUtils.decimalPoint s
    }

let decimalValuePrinter (ty:Type) =
    {
    finder = ty = typeof<decimal>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<decimal> value
        Convert.ToString value + "M"
    }

let nativeintValuePrinter (ty:Type) =
    {
    finder = ty = typeof<nativeint>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<nativeint> value
        Convert.ToString value + "n"
    }

let unativeintValuePrinter (ty:Type) =
    {
    finder = ty = typeof<unativeint>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<unativeint> value
        Convert.ToString value + "un"
    }
    
let bigintValuePrinter (ty:Type) =
    {
    finder = ty = typeof<bigint>
    print = fun loop (value:obj) (precContext:int) ->
        let value = unbox<bigint> value
        Convert.ToString value + "I"
    }

let unitValuePrinter (ty:Type) =
    {
    finder = ty = typeof<unit>
    print = fun loop (value:obj) (precContext:int) -> "()"
    }

let DBNullValuePrinter (ty:Type) =
    {
    finder = ty = typeof<DBNull>
    print = fun loop (value:obj) (precContext:int) ->
        "DBNull.Value"
    }

let GuidValuePrinter (ty:Type) =
    {
    finder = ty = typeof<Guid>
    print = fun loop (value:obj) (precContext:int) ->
        let id = unbox<Guid> value
        sprintf "Guid(\"%s\")" <| id.ToString()
        |> putParen precContext valuePrecedences.[" "]
    }

let enumValuePrinter (ty:Type) =
    {
    finder = ty.IsEnum
    print = fun loop (value:obj) (precContext:int) ->
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

let timeSpanValuePrinter (ty:Type) =
    {
    finder = ty = typeof<TimeSpan>
    print = fun loop (value:obj) (precContext:int) ->
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

let dateTimeOffsetValuePrinter (ty:Type) =
    {
    finder = ty = typeof<DateTimeOffset>
    print = fun loop (value:obj) (precContext:int) ->
        let thisDate = unbox<DateTimeOffset> value
        [
            thisDate.Year       .ToString()
            thisDate.Month      .ToString()
            thisDate.Day        .ToString()
            thisDate.Hour       .ToString()
            thisDate.Minute     .ToString()
            thisDate.Second     .ToString()
            thisDate.Millisecond.ToString()
            thisDate.Offset |> loop typeof<TimeSpan> <| valuePrecedences.[","]
        ]
        |> String.concat ","
        |> sprintf "DateTimeOffset(%s)"
        |> putParen precContext valuePrecedences.[" "]
    }

let dateTimeValuePrinter (ty:Type) =
    {
    finder = ty = typeof<DateTime>
    print = fun loop (value:obj) (precContext:int) ->
        let dt = unbox<DateTime> value
        loop typeof<DateTimeOffset> (DateTimeOffset dt) precContext
    }
    
let nullableValuePrinter (ty:Type) =
    {
    finder = 
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    print = fun loop (value:obj) (precContext:int) ->
        if value = null then
            "Nullable()"
        else
            let underlyingType = ty.GenericTypeArguments.[0]
            loop underlyingType value valuePrecedences.[" "]
            |> sprintf "Nullable %s"
        |> putParen precContext valuePrecedences.[" "]
    }

let arrayValuePrinter (ty:Type) =
    {
    finder =
        ty.IsArray && ty.GetArrayRank() = 1

    print = fun loop value prec ->
        let elemType = ty.GetElementType()
        let reader = ArrayType.toArray // ty
        let elements = reader value

        elements
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "[|%s|]"  //一定無需加括號
    
    }

let listValuePrinter (ty:Type) =
    { 
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition()
    print = fun loop (value:obj) (precContext:int) ->
        //let reader = ListType.readList ty
        let elemType = ty.GenericTypeArguments.[0]
        let elements = IEnumerableType.toArray value

        elements
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "[%s]" //一定無需加括號
    }

let setValuePrinter (ty:Type) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
    print = fun loop value precContext ->
        //let reader = SetType.readSet ty
        let elemType = ty.GenericTypeArguments.[0]
        let elements = IEnumerableType.toArray value

        elements
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "set [%s]"
        |> putParen precContext valuePrecedences.[" "]
    }

let hashsetValuePrinter (ty:Type) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<System.Collections.Generic.HashSet<_>>.GetGenericTypeDefinition()
    print = fun loop (value:obj) (precContext:int) ->
        let elements = 
            value
            |> IEnumerableType.toArray

        let elemType = ty.GenericTypeArguments.[0]

        elements
        |> Array.map(fun x -> loop elemType x valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "HashSet [%s]"
        |> putParen precContext valuePrecedences.[" "]

    }

let mapValuePrinter (ty:Type) =
    {
    finder =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>>
    print = fun loop (value:obj) (precContext:int) ->
        let reader = MapType.toArray ty
        let elements = reader value
        let kvty = elements.GetType().GetElementType()

        elements
        |> Array.map(fun kvp -> loop kvty kvp valuePrecedences.[";"])
        |> String.concat ";"
        |> sprintf "Map [%s]"
        |> putParen precContext valuePrecedences.[" "]
    }

let tupleValuePrinter (ty:Type) =
    {
        finder = FSharpType.IsTuple ty
        print = fun loop (value:obj) (precContext:int) ->
            let reader = TupleType.readTuple ty
            let fields = reader value

            fields
            |> Array.map(fun(ftype,field)-> loop ftype field valuePrecedences.[","])
            |> String.concat ","
            |> putParen precContext valuePrecedences.[","]
    }

let unionValuePrinter (ty:Type) =
    {
        finder = FSharpType.IsUnion ty
        print = fun loop (value:obj) (precContext:int) ->
            let reader = UnionType.readUnion ty
            let name,fields = reader value
            let qa = UnionType.getQualifiedAccess ty
            let name = qa + name

            match fields with
            | [||] -> name
            | [|ftype,field|] ->
                let payload = loop ftype field valuePrecedences.[" "]
                if payload.StartsWith("(") then name + payload else name + " " + payload
                |> putParen precContext valuePrecedences.[" "]
            | _ ->
                fields
                |> Array.map(fun(ftype,field)-> loop ftype field valuePrecedences.[","])
                |> String.concat ","
                |> sprintf "%s(%s)" name
                |> putParen precContext valuePrecedences.[" "]
    }

let recordValuePrinter (ty:Type) =
    {
        finder = FSharpType.IsRecord ty
        print = fun loop (value:obj) (precContext:int) ->
            let reader = RecordType.readRecord ty
            let fields = reader value

            fields
            |> Array.map(fun(pi,value)->
                let nm = 
                    if FSharpCodeUtils.isIdentifier pi.Name then
                        pi.Name 
                    else String.Format("``{0}``",pi.Name)

                let value = loop pi.PropertyType value 0
                String.Format("{0}= {1}",nm,value)
            )
            |> String.concat ";"
            |> sprintf "{%s}"
    }

let TypeValuePrinter ty = {
    finder = ty = typeof<Type>
    print = fun loop value prec ->
        value
        |> unbox<Type>
        |> TypePrinterUtils.typeStringify TypePrinterUtils.printers <| 0
        |> sprintf "typeof<%s>"
    }
