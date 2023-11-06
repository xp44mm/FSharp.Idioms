namespace FSharp.Idioms.EqualityCheckers

open System

type EqualityChecker = {
    check: bool
    equal: (Type -> obj -> obj -> bool) -> obj -> obj -> bool
}

