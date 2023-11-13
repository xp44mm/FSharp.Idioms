module FSharp.Idioms.EqualityCheckers.EqResolves

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection

open FSharp.Idioms
open FSharp.Reflection

let PrimitiveEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
    if ty.IsPrimitive
        || ty = typeof<DateTimeOffset>
        || ty = typeof<TimeSpan>
        || ty = typeof<bigint>
    then Some(fun loop -> (=))
    else None
    )

let DateTimeEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
    if ty = typeof<DateTime> then 
        Some(fun loop x y ->
            DateTime.Equals(unbox<DateTime>x,unbox<DateTime>y)
        )
    else None
    )

let DecimalEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
    if ty = typeof<Decimal> then 
        Some(fun loop x y ->
            Decimal.Equals(unbox<Decimal>x,unbox<Decimal>y)
        )
    else None
    )

let StringEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty = typeof<string> then 
            Some(fun loop x y ->
                let x = unbox<string> x
                let y = unbox<string> y
                System.String.Equals(x, y, StringComparison.Ordinal)
            )
        else None
    )

let TypeEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty = typeof<Type> then 
            Some(fun loop x y -> 
                let x = unbox<Type> x
                let y = unbox<Type> y
                Type.op_Equality(x,y))
        else None
    )

let UnitEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty = typeof<unit> then
            Some(fun loop x y -> true)
        else None
    )

let DBNullEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty = typeof<DBNull> then
            Some(fun loop x y -> true)
        else None
    )

let EnumEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsEnum then    
            //let uty = ty.GetEnumUnderlyingType()
            //Some(fun (loop:Type->obj->obj->bool) (x:obj) (y:obj) ->
            //    loop uty x y
            //)
            Some(fun loop -> (=))
        else None
    )

let NullableEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Nullable<_>>.GetGenericTypeDefinition() then
            //let elemType = ty.GenericTypeArguments.[0]
            //Some(fun loop x y ->
            //    match x,y with
            //    | null,null -> true
            //    | null,_ | _,null -> false
            //    | _ -> loop elemType x y
            //)
            Some(fun loop -> (=))
        else None
    )

let ArrayEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsArray && ty.GetArrayRank() = 1 then
            let reader = ArrayType.toArray // ty
            let elementType = ty.GetElementType()
            Some(fun loop x y -> 
            let ex = reader x
            let ey = reader y

            if ex.Length = ey.Length then
                Array.zip ex ey
                |> Array.forall(fun(b1,b2) -> loop b1 b2)
            else false
            )
        else None
    )

let TupleEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if FSharpType.IsTuple ty then
            let reader = FSharpValue.PreComputeTupleReader ty
            //let elementTypes = FSharpType.GetTupleElements ty

            Some(fun loop x y -> 
                let a1 = reader x
                let a2 = reader y
                Array.zip a1 a2
                |> Array.forall(fun(a1,a2) -> loop a1 a2)
            )
        else None
    )

let RecordEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if FSharpType.IsRecord ty then
            let reader = FSharpValue.PreComputeRecordReader ty
            //let fields = 
            //    FSharpType.GetRecordFields ty
            //    |> Array.map(fun pi -> pi.PropertyType)
            Some(fun loop x y ->
                let a1 = reader x
                let a2 = reader y
                Array.zip a1 a2
                |> Array.forall(fun(a1,a2) -> loop a1 a2)
            )
        else None
    )

let ListEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<list<_>>.GetGenericTypeDefinition() then
            let get_IsEmpty = ty.GetMethod("get_IsEmpty")
            //let get_IsCons = ty.GetMethod("get_IsCons")
            let get_Head = ty.GetMethod("get_Head")
            let get_Tail = ty.GetMethod("get_Tail")
            let get_IsEmpty ls = get_IsEmpty.Invoke(ls,[||]) |> unbox<bool>
            //let get_IsCons ls = get_IsCons.Invoke(ls,[||]) |> unbox<bool>
            let get_Head ls = get_Head.Invoke(ls,[||])
            let get_Tail ls = get_Tail.Invoke(ls,[||])
            //let elementType = ty.GenericTypeArguments.[0]
            let rec lsEqual elemEq lsx lsy =
                match get_IsEmpty lsx,get_IsEmpty lsy with
                | true,true -> true
                | true,false | false,true -> false
                | false,false ->
                    let xh = get_Head lsx
                    let xt = get_Tail lsx
                    let yh = get_Head lsy
                    let yt = get_Tail lsy
                    elemEq xh yh && lsEqual elemEq xt yt

            Some(fun loop x y ->
                lsEqual loop x y
            )
        else None
    )

let SetEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Set<_>>.GetGenericTypeDefinition() then
            //let reader = SetType.readSet ty
            //let elementType = ty.GenericTypeArguments.[0]
            Some(fun loop x y -> 
                let a1 = IEnumerableType.toArray x
                let a2 = IEnumerableType.toArray y
                if a1.Length = a2.Length then
                    Array.zip a1 a2
                    |> Array.forall(fun(a1,a2) -> loop a1 a2)
                else false
            )
        else None
    )

let MapEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<Map<_,_>>.GetGenericTypeDefinition() then
            let reader = MapType.toArray ty
            Some(fun loop (x:obj) (y:obj) -> 
                let a1 = reader x
                let a2 = reader y
                if a1.Length = a2.Length then
                    Array.zip a1 a2
                    |> Array.forall(fun(a1,a2) -> loop (box a1) (box a2))
                else false
            )
        else None
    )

let UnionEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if FSharpType.IsUnion ty then
            let tagReader = FSharpValue.PreComputeUnionTagReader ty
            let unionCases =
                FSharpType.GetUnionCases ty
                |> Array.map(fun uc ->
                    //let fieldTypes =
                    //    uc.GetFields()
                    //    |> Array.map(fun pi -> pi.PropertyType)
                    FSharpValue.PreComputeUnionReader uc
                    )

            Some(fun loop x y -> 
                let tag1 = tagReader x
                let tag2 = tagReader y
                if tag1 = tag2 then
                    let caseReader = unionCases.[tag1]
                    let objs1 = caseReader x
                    let objs2 = caseReader y
                    Array.zip objs1 objs2
                    |> Array.forall(fun(obj1,obj2)-> loop obj1 obj2)
                else false
            )
        else None
    )

let HashSetEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        if ty.IsGenericType && ty.GetGenericTypeDefinition() = typeof<HashSet<_>>.GetGenericTypeDefinition() then
            let mi = ty.GetMethod("SetEquals", BindingFlags.Instance ||| BindingFlags.Public)
            Some(fun loop x y ->
                mi.Invoke(x, [|y|])
                |> unbox<bool>
            )
        else None
    )

let op_EqualityEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let mi = ty.GetMethod("op_Equality", BindingFlags.Static ||| BindingFlags.Public)
        if mi <> null then
            Some(fun loop x y -> 
                mi.Invoke(null, [|x;y|])
                |> unbox<bool>
            )
        else None
    )

let SeqEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    let iname = "IEnumerable"
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ity = ty.GetInterface(iname)
        if ity <> null then
            Some(fun loop x y -> 
                NotImplementedException($"{iname} interface of {ty}")
                |> raise
            )
        else None
    )

let IEquatableEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    let iname = "IEquatable`1"
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ity = ty.GetInterface(iname)
        if ity <> null then
            Some(fun loop x y -> 
                NotImplementedException($"{iname} interface of {ty}")
                |> raise
            )
        else None
    )

let IComparableEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    let iname = "IComparable"
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ity = ty.GetInterface(iname)
        if ity <> null then
            Some(fun loop x y -> 
                NotImplementedException($"{iname} interface of {ty}")
                |> raise
            )
        else None
    )

let IStructuralEquatableEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    let iname = "IStructuralEquatable"
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ity = ty.GetInterface(iname)
        if ity <> null then
            Some(fun loop x y -> 
                NotImplementedException($"{iname} interface of {ty}")
                |> raise
            )
        else None
    )

let IStructuralComparableEqualityResolve =
    let memo = ConcurrentDictionary<Type,option<LoopEQ->obj->obj->bool>>(HashIdentity.Structural)
    let iname = "IStructuralComparable"
    fun (ty:Type) -> memo.GetOrAdd(ty,
        let ity = ty.GetInterface(iname)
        if ity <> null then
            Some(fun loop x y -> 
                NotImplementedException($"{iname} interface of {ty}")
                |> raise
            )
        else None
    )




