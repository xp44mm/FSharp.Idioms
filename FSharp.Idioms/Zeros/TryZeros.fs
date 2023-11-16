module FSharp.Idioms.Zeros.TryZeros

open System
open System.Reflection
open FSharp.Idioms
open FSharp.Reflection

type Loop = Type -> obj


let tryBool =
    fun (ty:Type) -> 
    if ty=typeof<bool> then
        Some(fun loop -> box false)
    else None

let trysbyte =
    fun (ty:Type) ->
    if ty=typeof<sbyte>
    then Some(fun loop -> box 0y
    ) else None
let tryint16 =
    fun (ty:Type) ->
    if ty=typeof<int16>
    then Some(fun loop -> box 0s
    ) else None
let tryint =
    fun (ty:Type) ->
    if ty=typeof<int>
    then Some(fun loop -> box 0
    ) else None
let tryint64 =
    fun (ty:Type) ->
    if ty=typeof<int64>
    then Some(fun loop -> box 0L
    ) else None
let trynativeint =
    fun (ty:Type) ->
    if ty=typeof<nativeint>
    then Some(fun loop -> box 0n
    ) else None
let trybyte =
    fun (ty:Type) ->
    if ty=typeof<byte>
    then Some(fun loop -> box 0uy
    ) else None
let tryuint16 =
    fun (ty:Type) ->
    if ty=typeof<uint16>
    then Some(fun loop -> box 0us
    ) else None
let trychar =
    fun (ty:Type) ->
    if ty=typeof<char>
    then Some(fun loop -> box '\u0000'
    ) else None
let tryuint32 =
    fun (ty:Type) ->
    if ty=typeof<uint32>
    then Some(fun loop -> box 0u
    ) else None
let tryuint64 =
    fun (ty:Type) ->
    if ty=typeof<uint64>
    then Some(fun loop -> box 0UL
    ) else None
let tryunativeint =
    fun (ty:Type) ->
    if ty=typeof<unativeint>
    then Some(fun loop -> box 0un
    ) else None
let trydecimal =
    fun (ty:Type) ->
    if ty=typeof<decimal>
    then Some(fun loop -> box 0M
    ) else None
let tryfloat =
    fun (ty:Type) ->
    if ty=typeof<float>
    then Some(fun loop -> box 0.0
    ) else None
let tryfloat32 =
    fun (ty:Type) ->
    if ty=typeof<float32>
    then Some(fun loop -> box 0.0f
    ) else None
let trybigint =
    fun (ty:Type) ->
    if ty=typeof<bigint>
    then Some(fun loop -> box 0I
    ) else None
let trystring =
    fun (ty:Type) ->
    if ty=typeof<string>
    then Some(fun loop -> box ""
    ) else None
//let tryBool =
//    fun (ty:Type) ->
//    finder = ty = typeof<Guid>
//    getDefault = fun (loop: Type -> obj) ->
//        Guid() |> box
//    ) else None
let tryDBNull =
    fun (ty:Type) ->
    if ty = typeof<DBNull> 
    then Some( fun (loop: Type -> obj) ->
        box DBNull.Value
    ) else None
let tryDateTimeOffset =
    fun (ty:Type) ->
    if ty = typeof<DateTimeOffset>
    then Some( fun (loop: Type -> obj) ->
        box DateTimeOffset.MinValue
    ) else None
let tryTimeSpan =
    fun (ty:Type) ->
    if ty = typeof<TimeSpan>
    then Some ( fun (loop: Type -> obj) -> 
        box TimeSpan.Zero
    ) else None
//let tryBool =
//    fun (ty:Type) ->
//    finder = ty = typeof<Uri>
//    getDefault = fun (loop: Type -> obj) -> 
//        box (Uri("http://www.contoso.com/"))
//    ) else None

let tryEnum =
    fun (ty:Type) ->
    if ty.IsEnum then
        Some(fun loop -> 
        Enum.GetNames(ty)
        |> Array.map(fun name ->
            ty.GetField(
                name, BindingFlags.Public ||| BindingFlags.Static
                ).GetValue(null)
        )
        |> Array.minBy(hash)
        )
    else None

//let EnumDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) ->
//    ) else None

let tryNullable =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Nullable<_>>.GetGenericTypeDefinition() then
        Some(fun loop -> 
        null
        )
    else None

//let NullableDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> 
//    ) else None

let tryOption =
    fun (ty:Type) ->
    if FSharpType.IsUnion ty && ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Option<_>>.GetGenericTypeDefinition() then
        Some(fun loop -> 
        null
        )
    else None

//let OptionDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> null
//    ) else None

//动态创建泛型数组：
//需要使用`Array.CreateInstance(elementType,length)`方式创建一个新的数组。elementType为数组元素的类型，length为数组的长度。
//然后使用Array.SetValue(object,index)方法将元素插入

let tryArray =
    fun (ty:Type) ->
    if ty.IsArray && ty.GetArrayRank() = 1 then
        Some(fun loop -> 
        Array.CreateInstance(ty.GetElementType(), 0)
        |> box
        )
    else None

//let ArrayDefaultValueGetter (ty:Type) =
//    {
//    finder = ty.Is
//    getDefault = fun (loop: Type -> obj) ->
//    ) else None
let tryList =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition() then
        Some(fun loop ->
        ListType.empty ty
        )
    else None

//let DefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) ->
//    ) else None

let trySet =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Set<_>>.GetGenericTypeDefinition() then
        Some(fun loop ->
        SetType.empty ty
        )
    else None

//let SetDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> 
//    ) else None
let tryMap =
    fun (ty:Type) ->
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Map<_,_>>.GetGenericTypeDefinition() then
        Some(fun loop ->
        MapType.empty ty
        )
    else None

//let MapDefaultValueGetter (ty:Type) =
//    {
//    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<>
//    getDefault = fun (loop: Type -> obj) ->
//        let emptyGeneric = MapType.mapModuleType.GetMethod "Empty"
//        let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
//        empty.Invoke(null, [||]) 
//    ) else None
let tryTuple =
    fun (ty:Type) ->
    if FSharpType.IsTuple ty then
        Some(fun loop ->
        let elementTypes = FSharpType.GetTupleElements(ty)
        let values =
            elementTypes
            |> Array.map(fun ety -> loop ety)
        FSharpValue.MakeTuple(values,ty)
        )
    else None

//let TupleDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> 
//    ) else None
let tryRecord =
    fun (ty:Type) ->
    if FSharpType.IsRecord ty then
        Some(fun loop ->
        let fields = FSharpType.GetRecordFields(ty)
        let values =
            fields
            |> Array.map (fun pi -> loop pi.PropertyType)
        FSharpValue.MakeRecord(ty, values)
        )
    else None

//let RecordDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> 
//    ) else None
let tryUnion =
    fun (ty:Type) ->
    if FSharpType.IsUnion ty then
        Some(fun loop ->
        let unionCaseInfo =
            FSharpType.GetUnionCases(ty)
            |> Array.minBy(fun i -> i.Tag)

        let uionFieldTypes =
            unionCaseInfo.GetFields()
            |> Array.map(fun info -> info.PropertyType)

        let fields =
            uionFieldTypes
            |> Array.map loop

        FSharpValue.MakeUnion(unionCaseInfo, fields)
        )
    else None
    
//let UnionDefaultValueGetter (ty:Type) =
//    {
//    finder = 
//    getDefault = fun (loop: Type -> obj) -> 
//    ) else None

let tryZero =
    fun (ty:Type) ->
    match ty.GetProperty("Zero", BindingFlags.Static ||| BindingFlags.Public) with
    | null -> None
    | pinfo -> Some(fun loop ->
        pinfo.GetValue(null, [||])
        )

//let fallbackDefaultValueGetter (ty:Type) =
//    {
//    finder = true
//    getDefault = fun (loop: Type -> obj) -> 
//        try
//            let pinfo =
                
//        with _ -> null
//    ) else None
