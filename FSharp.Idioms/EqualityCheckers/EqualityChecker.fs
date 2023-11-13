namespace FSharp.Idioms.EqualityCheckers

open System

type EqualityChecker = {
    check: bool
    equal: (Type -> obj -> obj -> bool) -> obj -> obj -> bool
}

type LoopEQ = obj -> obj -> bool

type EqResolve = LoopEQ -> obj -> obj -> bool

//type EqCase = Type EqResolver<EqResolve>
