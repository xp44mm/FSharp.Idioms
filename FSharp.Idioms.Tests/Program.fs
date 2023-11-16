module FSharp.Idioms.Program

open System
open System.IO
open System
open System.Collections
open System.Collections.Generic

open FSharp.Idioms.Literals
open System
open System.Xml
open System.Reflection

//Console.WriteLine($"{ty.IsAssignableTo ity} // ty.IsAssignableTo ity")
//Console.WriteLine($"{ty.IsAssignableFrom ity} // ty.IsAssignableFrom ity")

//Console.WriteLine($"{ity.IsAssignableTo ty} // ity.IsAssignableTo ty")
//Console.WriteLine($"{ity.IsAssignableFrom ty} // ity.IsAssignableFrom ty")

let [<EntryPoint>] main _ =
    let ty = typeof<Map<int,string>>
    //for m in ty.GetConstructors() do
    //    Console.WriteLine($"{m}")
    let ctor = ty.GetConstructors().[0]

    let ps = ctor.GetParameters()
    for p in ps do
        Console.WriteLine($"{p}")
    let pty =
        ps.[0].ParameterType
    Console.WriteLine($"{pty}")

    let etys = pty.GenericTypeArguments
    for ety in etys do
        Console.WriteLine($"{ety}")

    //let y = ctor.Invoke([|Array.CreateInstance(ty.GenericTypeArguments.[0], 0) |])
    //Console.WriteLine($"{y}")

    //let get_Empty = ty.GetMethod("get_Empty")
    //Console.WriteLine(sprintf "%A" get_Empty.IsStatic)
    //let y = get_Empty.Invoke(null,[||])
    //Console.WriteLine($"{y.GetType()},{y=( [] : list<int>)}")
    0
