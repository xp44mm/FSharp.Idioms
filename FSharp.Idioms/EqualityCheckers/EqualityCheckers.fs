﻿module FSharp.Idioms.EqualityCheckers.EqualityCheckers

open System
open System.Reflection

open FSharp.Idioms
open FSharp.Reflection

let AtomEqualityChecker (ty:Type) =
    {
    check = ty.IsPrimitive
        || ty = typeof<string>
        || ty = typeof<Type>
        || ty = typeof<decimal>
        || ty = typeof<bigint>
        || ty = typeof<DateTime>
        || ty = typeof<DateTimeOffset>
        || ty = typeof<TimeSpan>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) -> x = y
    }

let EnumEqualityChecker (ty:Type) =
    {
    check = ty.IsEnum
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let uty = ty.GetEnumUnderlyingType()
        loop uty x y
    }

let UnitEqualityChecker (ty:Type) =
    {
    check = ty = typeof<unit>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) -> true
    }

let DBNullEqualityChecker (ty:Type) =
    {
    check = ty = typeof<DBNull>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) -> true
    }

let NullableEqualityChecker (ty:Type) =
    {
    check = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        match x,y with
        | null,null -> true
        | null,_ | _,null -> false
        | _ ->
            let elemType = ty.GenericTypeArguments.[0]
            loop elemType x y
    }

let ArrayEqualityChecker (ty:Type) =
    {
    check = ty.IsArray && ty.GetArrayRank() = 1
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        match x,y with
        | null,null -> true
        | null,_ | _,null -> false
        | _ ->
            let reader = ArrayType.readArray ty
            let _, a1 = reader x
            let _, a2 = reader y

            if a1.Length = a2.Length then
                let elementType = ty.GetElementType()
                let loopElement = loop elementType

                Array.zip a1 a2
                |> Array.forall(fun(b1,b2) -> loopElement b1 b2)
            else false

    }

let TupleEqualityChecker (ty:Type) =
    {
    check = FSharpType.IsTuple ty
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let reader = TupleType.readTuple ty
        let elementTypes = FSharpType.GetTupleElements ty
        let a1 = reader x |> Array.map(snd)
        let a2 = reader y |> Array.map(snd)
        Array.zip3 elementTypes a1 a2
        |> Array.forall(fun(elemType,a1,a2) -> loop elemType a1 a2)
    }

let RecordEqualityChecker (ty:Type) =
    {
    check = FSharpType.IsRecord ty
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let reader = FSharpValue.PreComputeRecordReader ty
        let fields = FSharpType.GetRecordFields ty
        let a1 = reader x
        let a2 = reader y
        Array.zip3 fields a1 a2
        |> Array.forall(fun(pi,a1,a2) -> loop pi.PropertyType a1 a2)
    }

let ListEqualityChecker (ty:Type) =
    {
    check = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<list<_>>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let readlist = ListType.readList ty
        let _,x = readlist x
        let _,y = readlist y
        if x.Length = y.Length then
            let elementType = ty.GenericTypeArguments.[0]
            let loopElement = loop elementType
            Array.zip x y
            |> Array.forall(fun(e1,e2)-> loopElement e1 e2)
        else false
    }

let SetEqualityChecker (ty:Type) =
    {
    check = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Set<_>>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let reader = SetType.readSet ty
        let elementType = ty.GenericTypeArguments.[0]
        let loopElement = loop elementType

        let _, a1 = reader x
        let _, a2 = reader y
        if a1.Length = a2.Length then
            Array.zip a1 a2
            |> Array.forall(fun(a1,a2) -> loopElement a1 a2)
        else false
    }

let MapEqualityChecker (ty:Type) =
    {
    check = ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Map<_,_>>
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let reader = MapType.readMap ty
        let _, a1 = reader x
        let _, a2 = reader y
        if a1.Length = a2.Length then
            let elementType = FSharpType.MakeTupleType(ty.GenericTypeArguments)
            let loopElement = loop elementType
            Array.zip a1 a2
            |> Array.forall(fun(a1,a2) -> loopElement a1 a2)
        else false

    }

let UnionEqualityChecker (ty:Type) =
    {
    check = FSharpType.IsUnion ty
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        let unionCases = FSharpType.GetUnionCases ty
        let unionCases =
            unionCases
            |> Array.map(fun uc ->
                let fieldTypes =
                    uc.GetFields()
                    |> Array.map(fun pi -> pi.PropertyType)
                {|
                    name =  uc.Name
                    fieldTypes = fieldTypes
                    reader = FSharpValue.PreComputeUnionReader uc
                |})

        let tagReader = FSharpValue.PreComputeUnionTagReader ty
        let tag1 = tagReader x
        let tag2 = tagReader y
        if tag1 = tag2 then
            let uc = unionCases.[tag1]
            let objs1 = uc.reader x
            let objs2 = uc.reader y
            Array.zip3 uc.fieldTypes objs1 objs2
            |> Array.forall(fun(fty,obj1,obj2)-> loop fty obj1 obj2)
        else false
    }

let op_EqualityEqualityChecker (ty:Type) =
    let mi = ty.GetMethod("op_Equality", BindingFlags.Static ||| BindingFlags.Public)
    {
    check = mi <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        mi.Invoke(null, [|x;y|])
        |> unbox<bool>
    }

let SeqEqualityChecker (ty:Type) =
    let iname = "IEnumerable"
    {
    check = ty.GetInterface(iname) <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        NotImplementedException( $"<{iname}>:{ty}" )
        |> raise
    }

let IEquatableEqualityChecker (ty:Type) =
    let iname = "IEquatable`1"
    {
    check = ty.GetInterface(iname) <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        NotImplementedException($"<{iname}>:{ty}")
        |> raise
    }

let IComparableEqualityChecker (ty:Type) =
    let iname = "IComparable"
    {
    check = ty.GetInterface(iname) <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        NotImplementedException($"<{iname}>:{ty}")
        |> raise
    }

let IStructuralEquatableEqualityChecker (ty:Type) =
    let iname = "IStructuralEquatable"
    {
    check = ty.GetInterface(iname) <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        NotImplementedException($"<{iname}>:{ty}")
        |> raise
    }

let IStructuralComparableEqualityChecker (ty:Type) =
    let iname = "IStructuralComparable"
    {
    check = ty.GetInterface(iname) <> null
    equal = fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
        NotImplementedException($"{iname} interface of {ty}")
        |> raise
    }



