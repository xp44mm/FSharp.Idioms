namespace FSharp.Idioms.Literals
open System

/// int -> Type -> 
type TypePrinter = {
    finder: bool
    print: (int -> Type -> string) -> string
}
