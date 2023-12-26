module FSharp.Idioms.Zeros.TryZeros

open System
open System.Reflection
open FSharp.Idioms
open FSharp.Reflection

type Loop = Type -> obj

let tryBool = 
    let x = box false    
    fun (ty:Type) ->
    if ty=typeof<bool> then
        Some(fun (loop:Loop) -> x)
    else None

let trySbyte = 
    let x = box 0y
    fun (ty:Type) ->
    if ty=typeof<sbyte> then
        Some(fun (loop:Loop) -> x)
    else None

let tryInt16 = 
    let s = box 0s
    fun (ty:Type) ->
    if ty=typeof<int16> then
        Some(fun (loop:Loop) -> s)
    else None

let tryInt =
    let x = box 0
    fun (ty:Type) ->
    if ty=typeof<int> then 
        Some(fun (loop:Loop) -> x)
    else None

let tryInt64 = 
    let x = box 0L
    fun (ty:Type) ->
    if ty=typeof<int64> then Some(fun (loop:Loop) -> x) else None

let tryNativeint = 
    let x = box 0n
    fun (ty:Type) ->
    if ty=typeof<nativeint> then Some(fun (loop:Loop) -> x) else None

let tryByte = 
    let x = box 0uy 
    fun (ty:Type) ->
    if ty=typeof<byte> then Some(fun (loop:Loop) -> x) else None

let tryUint16 = 
    let x = box 0us 
    fun (ty:Type) ->
    if ty=typeof<uint16> then Some(fun (loop:Loop) -> x) else None

let tryChar = 
    let x = box '\u0000' 
    fun (ty:Type) ->
    if ty=typeof<char> then Some(fun (loop:Loop) -> x) else None

let tryUint32 = 
    let x = box 0u 
    fun (ty:Type) ->
    if ty=typeof<uint32> then Some(fun (loop:Loop) -> x) else None

let tryUint64 = 
    let x = box 0UL 
    fun (ty:Type) ->
    if ty=typeof<uint64> then Some(fun (loop:Loop) -> x) else None

let tryUnativeint = 
    let x = box 0un
    fun (ty:Type) ->
    if ty=typeof<unativeint> then Some(fun (loop:Loop) -> x ) else None

let tryDecimal = 
    let x = box 0M
    fun (ty:Type) ->
    if ty=typeof<decimal> then Some(fun (loop:Loop) -> x) else None

let tryFloat = 
    let x = box 0.0 
    fun (ty:Type) ->
    if ty=typeof<float> then Some(fun (loop:Loop) -> x) else None

let tryFloat32 = 
    let x = box 0.0f
    fun (ty:Type) ->
    if ty=typeof<float32> then Some(fun (loop:Loop) -> x) else None

let tryBigint = 
    let x = box 0I
    fun (ty:Type) ->
    if ty=typeof<bigint> then Some(fun (loop:Loop) -> x) else None

let tryString = 
    let x = box ""
    fun (ty:Type) ->
    if ty=typeof<string> then Some(fun (loop:Loop) -> x) else None

let tryDBNull = 
    let x = box DBNull.Value
    fun (ty:Type) ->
    if ty = typeof<DBNull> then Some( fun (loop: Type -> obj) -> x) else None

let tryDateTimeOffset = 
    let x = box DateTimeOffset.MinValue
    fun (ty:Type) ->
    if ty = typeof<DateTimeOffset> then Some( fun (loop: Type -> obj) -> x ) else None

let tryTimeSpan = 
    let x = box TimeSpan.Zero
    fun (ty:Type) ->
    if ty = typeof<TimeSpan> then Some(fun (loop: Type -> obj) -> x) else None

let tryEnum = 
    fun (ty:Type) ->
    if ty.IsEnum then 
        let value = Enum.GetValues(ty).GetValue 0
        let x = Enum.ToObject(ty,value)
        Some(fun (loop:Loop) -> x) 
    else None

let tryNullable = 
    let typedef = typeof<Nullable<_>>.GetGenericTypeDefinition()
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedef then
        Some(fun (loop:Loop) -> null : obj)
    else None

let tryOption = fun (ty:Type) ->
    if FSharpType.IsUnion ty && ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Option<_>>.GetGenericTypeDefinition() then
        Some(fun (loop:Loop) -> null : obj )
    else None

let tryArray = fun (ty:Type) ->
    if ty.IsArray && ty.GetArrayRank() = 1 then
        let ety = ty.GetElementType()
        let arr = Array.CreateInstance(ety, 0) |> box
        Some(fun (loop:Loop) -> arr)
    else None

let tryList = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition() then
        let empty = ListType.empty ty
        Some(fun (loop:Loop) -> empty)
    else None

let trySet = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Set<_>>.GetGenericTypeDefinition() then
        let empty = SetType.empty ty
        Some(fun (loop:Loop) -> empty)
    else None

let tryMap = fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Map<_,_>>.GetGenericTypeDefinition() then
        let empty = MapType.empty ty
        Some(fun (loop:Loop) -> empty)
    else None

let tryTuple = fun (ty:Type) ->
    if FSharpType.IsTuple ty then
        let elementTypes = FSharpType.GetTupleElements(ty)
        Some(fun (loop:Loop) ->
        let values =
            elementTypes
            |> Array.map(fun ety -> loop ety)
        FSharpValue.MakeTuple(values,ty)
        )
    else None

let tryRecord = fun (ty:Type) ->
    if FSharpType.IsRecord ty then
        let props = 
            FSharpType.GetRecordFields(ty)
            |> Array.map (fun pi -> pi.PropertyType)
        Some(fun (loop:Loop) ->
        let values =
            props
            |> Array.map (fun pty -> loop pty)
        FSharpValue.MakeRecord(ty, values)
        )
    else None

let tryUnion = fun (ty:Type) ->
    if FSharpType.IsUnion ty then
        let unionCaseInfo =
            FSharpType.GetUnionCases(ty)
            |> Array.minBy(fun i -> i.Tag)
        let uionFieldTypes =
            unionCaseInfo.GetFields()
            |> Array.map(fun info -> info.PropertyType)

        Some(fun (loop:Loop) ->
            let fields =
                uionFieldTypes
                |> Array.map loop
            FSharpValue.MakeUnion(unionCaseInfo, fields)
        )
    else None

let tryZero = fun (ty:Type) ->
    match ty.GetProperty("Zero", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic) with
    | null -> None
    | pinfo -> 
        let x = pinfo.GetValue(null, [||])
        Some(fun (loop:Loop) -> x)

let tryValueType = fun (ty:Type) ->
    if ty.IsValueType then
        let obj = Activator.CreateInstance ty
        Some(fun (loop:Loop) -> obj)
    else None

/// parameterless constructor
let tryCtor = fun (ty:Type) ->
    match 
        ty.GetConstructor(BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic, null, [||], null)
    with
    | null -> None
    | ctor -> 
        let obj = ctor.Invoke [||]
        Some(fun (loop:Loop) -> obj)


