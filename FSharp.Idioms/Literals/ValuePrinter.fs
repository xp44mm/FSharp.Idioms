namespace FSharp.Idioms.Literals
open System

/// Type -> 
type ValuePrinter = {
    finder: bool
    print: (Type -> obj -> int -> string) -> obj -> int -> string
}
