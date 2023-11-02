namespace FSharp.Idioms.Literals
open System

/// int -> Type -> obj -> 
type ValuePrinter = {
    finder: bool
    print: (int -> Type -> obj -> string) -> string
}
