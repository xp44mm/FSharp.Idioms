module FSharp.Idioms.Program

open System
open FSharp.Literals
open FSharp.Idioms.Memoization

let decorateMemo () =

    let fib myself n =
        printfn "computing fib %d" n
        if n <= 2 then 1 else myself (n - 1) + myself (n - 2)

    let fibonacci = memoize(fib)

    let y3 = fibonacci 3
    printfn "fibonacci %d" y3

    let y5 = fibonacci 5
    printfn "fibonacci %d" y5

    let y5 = fibonacci 5
    printfn "fibonacci %d" y5

let [<EntryPoint>] main _ = 
    decorateMemo ()
    0
