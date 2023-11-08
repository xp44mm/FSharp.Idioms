namespace FSharp.Idioms.Literals
open System

/// Type -> 
type TypePrinter = {
    finder: bool
    print: (Type -> int -> string) -> int -> string
}
