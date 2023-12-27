module FSharp.Idioms.EqualityComparers.EqualityComparerUtils

open System
open System.Collections

open FSharp.Idioms

let SetEqualsEquatableEq (ty:Type) =
    match ty.GetMethod("SetEquals") with
    | null -> None
    | mi ->
        let setEquals x y = mi.Invoke(x, [|y|])
        Some(fun comparer (x:obj,y:obj) ->
            setEquals x y
            |> unbox<bool>
        )

let IStructuralEquatableEq (ty:Type) =
    match ty.GetInterface("IStructuralEquatable") with
    | null -> None
    | ity ->
        let equals = ity.GetMethod("Equals")
        Some(fun comparer (x:obj,y:obj) ->
            equals.Invoke(x, [|y;comparer|])
            |> unbox<bool>
        )

let equalities = [
    IStructuralEquatableEq
    SetEqualsEquatableEq
    ]

let equalFn (equalities:list<Type->option<IEqualityComparer->obj*obj->bool>>) =
    let rec main (x:obj,y:obj) =
        match x,y with
        | null,null -> true
        | _,null | null,_ -> false
        | _ ->
            let xt = x.GetType()
            let yt = y.GetType()
            if xt = yt then
                let ty = xt
                let picked =
                    equalities
                    |> Seq.tryPick(fun checker -> checker ty)
                    |> Option.defaultValue(fun _ (x,y) -> x = y)
                let comparer = {
                    new IEqualityComparer with
                        member this.Equals(x:obj, y:obj) = main(x,y)
                        member this.GetHashCode(y:obj) =
                            StructuralComparisons.StructuralEqualityComparer.GetHashCode y
                }
                picked comparer (x,y)
            else false
    main

let equalObj:obj*obj->bool = equalFn equalities

let equal<'t> (x:'t,y:'t) = equalObj(x,y)
