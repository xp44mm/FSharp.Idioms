module FSharp.Idioms.Program

open System
open FSharp.Literals.Literal
open System.IO

let str a b c = sprintf "%s%s%s" a b c

//let x = ([]:int _)
//type type'<'a>() =
//    static member tostr(i:'a) =
//        match typeof<'a> with tp ->
//        if tp = typeof<int> then (Convert.ToString:int->_)(v)
//        elif tp = typeof<float> then Convert.ToString(v)

let [<EntryPoint>] main _ =
    0
