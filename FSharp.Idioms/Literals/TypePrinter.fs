namespace FSharp.Idioms.Literals
open System

/// read from fsharp value to json
type TypePrinter = 
    abstract filter: Type -> bool
    abstract print: loop:(int -> Type -> string) * prec:int * Type -> string

