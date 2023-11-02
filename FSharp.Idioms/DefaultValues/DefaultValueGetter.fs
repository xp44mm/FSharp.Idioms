namespace FSharp.Idioms.DefaultValues

open System

/// Type -> 
type DefaultValueGetter = {
    finder: bool
    getDefault: (Type -> obj) -> obj
}
