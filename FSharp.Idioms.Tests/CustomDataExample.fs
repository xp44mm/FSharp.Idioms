namespace FSharp.Idioms.EqualityCheckers

type ProductionCrew(production:list<string>,leftside:string,body:list<string>) =
    member _.production = production
    member _.leftside = leftside
    member _.body = body

open System
open FSharp.Idioms.EqualityCheckers
open System.Collections

module CustomDataExample =
    let ProductionCrewEq (ty:Type) =
        if ty = typeof<ProductionCrew> then
            Some(fun (loop:IEqualityComparer) (x, y) ->
            let x = unbox<ProductionCrew> x
            let y = unbox<ProductionCrew> y
            loop.Equals(x.production, y.production)  &&
            loop.Equals(x.leftside, y.leftside)      &&
            loop.Equals(x.body, y.body)
            )
        else None
    let eqs = ProductionCrewEq :: EqualityComparerUtils.equalities

    let equalObj (x:obj,y:obj) = EqualityComparerUtils.equalFn eqs (x,y)

    let equal<'t> (x:'t,y:'t) = EqualityComparerUtils.equalFn eqs (x,y)
