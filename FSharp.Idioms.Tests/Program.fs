module FSharp.Idioms.Program

open System
open FSharp.Literals

let [<EntryPoint>] main _ = 
    let map = Map [("NULL", set ["NULL"]); ("anonPattern", set ["NULL"])]
    let remains = set [
        ("anonPattern", "anonPatterns"); 
        ("anonPatterns", "anonPattern");
        //("pattern", "anonPattern")
        ]

    let y = Graph.propagate map remains
    Console.WriteLine(Literal.stringify y)
    0
