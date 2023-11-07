module FSharp.Idioms.EqualityCheckers.EqualityCheckerUtils

open System

open FSharp.Idioms
open FSharp.Idioms.EqualityCheckers.EqualityCheckers

let checkers = [
    AtomEqualityChecker
    EnumEqualityChecker
    UnitEqualityChecker
    DBNullEqualityChecker
    NullableEqualityChecker
    ArrayEqualityChecker
    TupleEqualityChecker
    RecordEqualityChecker
    ListEqualityChecker
    SetEqualityChecker
    MapEqualityChecker
    UnionEqualityChecker
    HashSetEqualityChecker
    op_EqualityEqualityChecker
    SeqEqualityChecker
    IEquatableEqualityChecker
    IComparableEqualityChecker
    IStructuralEquatableEqualityChecker
    IStructuralComparableEqualityChecker
]

let rec equalsFn (checkers:list<Type->EqualityChecker>) (ty:Type) =
    let picked =
        checkers
        |> Seq.tryPick(fun checker ->
            match checker ty with
            | {check=true;equal=equal} -> Some equal
            | {check=false} -> None
            )
        |> Option.defaultValue (fun loop x y ->
            NotImplementedException($"Option.defaultValue: {ty}")
            |> raise
        )
    picked (equalsFn checkers)

/// equals dynamic
let equalsDynamic (tp:Type) : (obj->obj->bool) = equalsFn checkers tp

/// print generic value
let equals<'t> (x:'t) (y:'t) = 
    equalsFn checkers typeof<'t> x y
