module FSharp.Idioms.EqualityCheckers.EqualUtils


open System

open FSharp.Idioms
open FSharp.Idioms.EqualityCheckers.EqResolves

//容器用
//IEqualityComparer
//IComparer

let resolves = [
    PrimitiveEqualityResolve
    TypeEqualityResolve
    UnitEqualityResolve
    DBNullEqualityResolve
    EnumEqualityResolve
    NullableEqualityResolve
    ArrayEqualityResolve
    TupleEqualityResolve
    RecordEqualityResolve
    ListEqualityResolve
    SetEqualityResolve
    MapEqualityResolve
    UnionEqualityResolve
    HashSetEqualityResolve
    op_EqualityEqualityResolve
    SeqEqualityResolve
    IEquatableEqualityResolve
    IComparableEqualityResolve
    IStructuralEquatableEqualityResolve
    IStructuralComparableEqualityResolve
]

open System.Collections.Concurrent

let rec equalFn (resolves:list<Type->option<EqResolve>>) = 
    let memo = ConcurrentDictionary<Type,obj->obj->bool>(HashIdentity.Structural)

    let fn (ty:Type) =
        let picked =
            resolves
            |> Seq.tryPick(fun checker -> checker ty )
            |> Option.defaultValue(fun loop x y ->
                NotImplementedException($"Option.defaultValue: {ty}")
                |> raise
            )
        picked (equalFn resolves)
    let main (x:obj) (y:obj) =
        match x,y with
        | null,null -> true
        | null,_ | _,null -> false
        | _ ->
            let xt = x.GetType()
            let yt = y.GetType()
            if xt = yt then
                let fn = memo.GetOrAdd(xt,fn xt)
                fn x y
            else false
    main

/// equals dynamic
let equalDynamic (x:obj) (y:obj) = equalFn resolves x y

/// equal generic value
let equal<'t> (x:'t) (y:'t) = equalFn resolves x y
