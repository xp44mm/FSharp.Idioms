module FSharp.Idioms.Jsons.JsonWriters // Json -> 'obj

open FSharp.Idioms
open FSharp.Idioms.Reflection
open FSharp.Reflection

open System
open System.Collections.Generic

type Loop = Type -> Json -> obj

let tryBool = fun (ty:Type) ->
    if ty = typeof<bool> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.True -> true
        | Json.False -> false
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryString = fun (ty:Type) ->
    if ty = typeof<string> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryChar = fun (ty:Type) ->
    if ty = typeof<char> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> x.[0]
        | _ -> failwith $"{json}"
        |> box)
    else None

let trySbyte = fun (ty:Type) ->
    if ty = typeof<sbyte> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> sbyte x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryByte = fun (ty:Type) ->
    if ty = typeof<byte> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> byte x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryInt16 = fun (ty:Type) ->
    if ty = typeof<int16> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> int16 x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryUint16 = fun (ty:Type) ->
    if ty = typeof<uint16> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> uint16 x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryInt = fun (ty:Type) ->
    if ty = typeof<int> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> int x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryUint32 = fun (ty:Type) ->
    if ty = typeof<uint32> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> uint32 x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryInt64 = fun (ty:Type) ->
    if ty = typeof<int64> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> Int64.Parse x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryUint64 = fun (ty:Type) ->
    if ty = typeof<uint64> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> UInt64.Parse x
        | _ -> failwith $"{json}"
        |> box)
    else None

let trySingle = fun (ty:Type) ->
    if ty = typeof<single> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> single x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryFloat = fun (ty:Type) ->
    if ty = typeof<float> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Number x -> float x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryDecimal = fun (ty:Type) ->
    if ty = typeof<decimal> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> Decimal.Parse x
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryNativeint = fun (ty:Type) ->
    if ty = typeof<nativeint> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> IntPtr(Int64.Parse x)
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryUnativeint = fun (ty:Type) ->
    if ty = typeof<unativeint> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.String x -> UIntPtr(UInt64.Parse x)
        | _ -> failwith $"{json}"
        |> box)
    else None

let tryUnit = fun (ty:Type) ->
    if ty = typeof<unit> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array [] -> null
        | _ -> failwith $"{json}"
        )
    else None

let tryDBNull = fun (ty:Type) ->
    if ty = typeof<DBNull> then
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Null -> box DBNull.Value
        | _ -> failwith $"{json}"
        )
    else None

let tryOption = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Option<_>>.GetGenericTypeDefinition() then
        let vty = ty.GenericTypeArguments.[0]
        let unionCase =
            FSharpType.GetUnionCases ty
            |> Array.find(fun c -> c.Name = "Some")
        Some(fun (loop:Loop) (json: Json) -> 
        match json with
        | Json.Array [] -> null
        | Json.Array [json] -> FSharpValue.MakeUnion(unionCase, [|loop vty json|])
        | _ -> failwith $"{json}"
        )
    else None

let tryNullable = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>> then
        let vty = ty.GenericTypeArguments.[0]
        Some(fun (loop:Loop) (json: Json) -> 
        match json with
        | Json.Null -> null
        | x -> loop vty x
        )
    else None

let tryEnum = fun (ty:Type) ->
    if ty.IsEnum then
        let enumUnderlyingType = EnumType.getEnumUnderlyingType ty
        let values = EnumType.getValues ty //enum id对应的背后类型

        if ty.IsDefined(typeof<FlagsAttribute>,false) then
            Some(fun (loop:Loop) (json: Json) -> 
            match json with
            | Json.Array flags ->
                let u =
                    flags
                    |> List.map(function
                        | Json.String flag -> values.[flag]
                        | json -> failwith $"{json}"
                    )
                    |> List.reduce(|||)
                    |> EnumType.fromUInt64 enumUnderlyingType
                Enum.ToObject(ty,u)
            | _ -> failwith $"{json}"
            )
        else
            Some(fun (loop:Loop) (json: Json) -> 
            match json with
            | Json.String enum ->
                let u =
                    values.[enum]
                    |> EnumType.fromUInt64 enumUnderlyingType
                Enum.ToObject(ty,u)
            | _ -> failwith $"{json}"
            )
    else None
            
let tryArray = fun (ty:Type) ->
    if ty.IsArray && ty.GetArrayRank() = 1 then
        let elementType = ty.GetElementType()
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            let arr = Array.CreateInstance(elementType, elements.Length)
            elements
            |> List.map(fun e -> loop elementType e)
            |> List.iteri(fun i v -> arr.SetValue(v, i))
            box arr
        | _ -> failwith $"{json}")
    else None

let tryTuple = fun (ty:Type) ->
    if FSharpType.IsTuple ty then
        let elementTypes = FSharpType.GetTupleElements ty
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            let values =
                elements
                |> Array.ofList
                |> Array.zip elementTypes
                |> Array.map (fun (ety, elem) -> loop ety elem)
            FSharpValue.MakeTuple(values, ty)
        | _ -> failwith $"{json}"
        )
    else None

let tryRecord = fun (ty:Type) ->
    if FSharpType.IsRecord ty then
        let pis = FSharpType.GetRecordFields ty
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Object _ ->
            let values =
                pis
                |> Array.map(fun pi -> 
                    if json.hasProperty pi.Name then
                        loop pi.PropertyType json.[pi.Name]
                    else 
                        Zeros.ZeroUtils.getZero Zeros.ZeroUtils.tries pi.PropertyType
                )
            FSharpValue.MakeRecord(ty,values)
        | _ -> failwith $"{json}")
    else None

let tryList = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition() then
        let ety = ty.GenericTypeArguments.[0]
        let cons = ListType.Cons ty
        let empty = ListType.empty ty
        
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            elements
            |> List.rev
            |> List.map(fun elem -> loop ety elem)
            |> Seq.fold(fun ls e -> cons(e,ls)) empty
        | _ -> failwith $"{json}"
        )
    else None



let trySet = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Set<_>>.GetGenericTypeDefinition() then
        let ety = ty.GenericTypeArguments.[0]
        let ctor = SetType.ctor ty
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            let arr = Array.CreateInstance(ety, elements.Length)
            elements
            |> List.map(fun e -> loop ety e)
            |> List.iteri(fun i v -> arr.SetValue(v, i))
            ctor arr
        | _ -> failwith $"{json}"
        )
    else None

let tryHashSet = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<System.Collections.Generic.HashSet<_>>.GetGenericTypeDefinition() then
        let ety = ty.GenericTypeArguments.[0]
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            let arr = Array.CreateInstance(ety, elements.Length)
            elements
            |> List.map(fun e -> loop ety e)
            |> List.iteri(fun i v -> arr.SetValue(v, i))
            Activator.CreateInstance(ty,arr)
        | _ -> failwith $"{json}"
        )
    else None

let tryMap = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Map<_,_>>.GetGenericTypeDefinition() then
        let kty = ty.GenericTypeArguments.[0]
        let vty = ty.GenericTypeArguments.[1]
        let pty = FSharpType.MakeTupleType ty.GenericTypeArguments
        Some(fun (loop:Loop) (json: Json) ->
        match json with
        | Json.Array elements ->
            let arr = Array.CreateInstance(pty, elements.Length)
            elements
            |> List.map(fun e -> 
                match e with
                | Json.Array [k;v] ->
                    let kk = loop kty k
                    let vv = loop vty v
                    FSharpValue.MakeTuple([|kk;vv|],pty)
                | _ -> failwith $"{e}"
                )
            |> List.iteri(fun i v -> arr.SetValue(v, i))
            Activator.CreateInstance(ty,arr)
        | _ -> failwith $"{json}"
        )
    else None

let tryUnion = fun (ty:Type) ->
    if FSharpType.IsUnion ty then
        let cases = 
            FSharpType.GetUnionCases(ty)
            |> Array.map(fun unionCaseInfo ->            
                let uionFieldTypes =
                    UnionType.getCaseFields unionCaseInfo
                    |> Array.map(fun info -> info.PropertyType)
                unionCaseInfo.Name,(unionCaseInfo,uionFieldTypes) // return
            )
            |> Map.ofArray

        Some(fun (loop:Loop) (json: Json) ->
            match json with
            | Json.String tag ->
                let unionCaseInfo,uionFieldTypes = cases.[tag]
                FSharpValue.MakeUnion(unionCaseInfo, [||])
            | Json.Array (name::fields) ->
                let tag = match name with Json.String tag -> tag | _ -> failwith ""
                let unionCaseInfo,uionFieldTypes = cases.[tag]
                //todo:根据类型信息截长补短？
                let fields =
                    fields
                    |> Array.ofList
                    |> Array.zip uionFieldTypes
                    |> Array.map(fun (t,j) -> loop t j)
                FSharpValue.MakeUnion(unionCaseInfo, fields)
            | _ -> failwith "Json structure does not match"
        )
    else None

