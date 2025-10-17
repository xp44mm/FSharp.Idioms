module FSharp.Idioms.Program

open System
open System.IO
open System.Text

open System
open System.Collections
open System.Collections.Generic

open FSharp.Idioms.Literal
open System
open System.Xml
open System.Reflection


open System
open System.IO
open System.Text

let [<EntryPoint>] main _ =
    let iterable = Seq.ofList [ 1..5 ]
    let iterator = CircularBufferIterator(iterable)

    //let e = [ 1; 2; 1; 2; 3 ]
    for _ in [1..12] do
        Console.WriteLine(stringify (iterator.tryNext()))

    Console.WriteLine("iterator.reset()")
    iterator.reset()
    for _ in [1..12] do
        Console.WriteLine(stringify (iterator.tryNext()))

    0
