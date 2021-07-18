module FSharp.Idioms.Program

open System
open FSharp.Literals

let [<EntryPoint>] main _ = 
    let x = 
        Array.create 0 0
        |> Array.mapi(fun i x -> i,x)
    Console.WriteLine(Render.stringify x)
    0
