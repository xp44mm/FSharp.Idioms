module FSharp.Idioms.DefaultValues.DefaultValueGetters

open System
open System.Reflection
open FSharp.Idioms
open FSharp.Reflection

let EnumDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsEnum
    getDefault = fun (loop: Type -> obj) ->
        Enum.GetNames(ty)
        |> Array.map(fun name ->
            ty.GetField(
                name, BindingFlags.Public ||| BindingFlags.Static
                ).GetValue(null)
        )
        |> Array.minBy(hash)
    }

let NullableDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    getDefault = fun (loop: Type -> obj) -> null
    }

let OptionDefaultValueGetter (ty:Type) =
    {
    finder = FSharpType.IsUnion ty && ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Option<_>>
    getDefault = fun (loop: Type -> obj) -> null
    }

//动态创建泛型数组：
//需要使用`Array.CreateInstance(elementType,length)`方式创建一个新的数组。elementType为数组元素的类型，length为数组的长度。
//然后使用Array.SetValue(object,index)方法将元素插入

let ArrayDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsArray
    getDefault = fun (loop: Type -> obj) ->
        let elementType = ty.GetElementType()
        Array.CreateInstance(elementType, 0) 
        |> box
    }

let ListDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<list<_>>
    getDefault = fun (loop: Type -> obj) ->
        let listModuleType = ListType.listModuleType
        let emptyGeneric = listModuleType.GetMethod "Empty"
        let listempty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
        listempty.Invoke(null, [||])
    }

let SetDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
    getDefault = fun (loop: Type -> obj) -> 
        let emptyGeneric = SetType.setModuleType.GetMethod "Empty"
        let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
        empty.Invoke(null, [||])
    }

let MapDefaultValueGetter (ty:Type) =
    {
    finder = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>>
    getDefault = fun (loop: Type -> obj) ->
        let emptyGeneric = MapType.mapModuleType.GetMethod "Empty"
        let empty = emptyGeneric.MakeGenericMethod(ty.GenericTypeArguments)
        empty.Invoke(null, [||]) 
    }

let TupleDefaultValueGetter (ty:Type) =
    {
    finder = FSharpType.IsTuple ty
    getDefault = fun (loop: Type -> obj) -> 
        let elementTypes = FSharpType.GetTupleElements(ty)
        let values =
            elementTypes
            |> Array.map(fun ety -> loop ety)
        FSharpValue.MakeTuple(values,ty)
    }

let RecordDefaultValueGetter (ty:Type) =
    {
    finder = FSharpType.IsRecord ty
    getDefault = fun (loop: Type -> obj) -> 
        let fields = FSharpType.GetRecordFields(ty)
        let values =
            fields
            |> Array.map (fun pi -> loop pi.PropertyType)
        FSharpValue.MakeRecord(ty, values)
    }
    
let UnionDefaultValueGetter (ty:Type) =
    {
    finder = FSharpType.IsUnion ty
    getDefault = fun (loop: Type -> obj) -> 
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
    }

let fallbackDefaultValueGetter (ty:Type) =
    {
    finder = true
    getDefault = fun (loop: Type -> obj) -> 
        try
            let pinfo =
                ty.GetProperty("Zero", BindingFlags.Static ||| BindingFlags.Public)
            pinfo.GetValue(null, null)
        with _ -> null
    }
