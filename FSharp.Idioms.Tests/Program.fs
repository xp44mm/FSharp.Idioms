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
    let ty = typeof<HashSet<Type>>
    //let mi = ty.GetMethod("SetEquals", BindingFlags.Instance ||| BindingFlags.Public)
    //Console.WriteLine($"{mi}")

    for itf in ty.GetInterfaces() do
        Console.WriteLine($"{itf.Name}")

    //let icty = ty.GetInterface("IComparable")
    //Console.WriteLine($"{icty}")

    //let mis = icty.GetMethods()
    //for mi in mis do
    //    Console.WriteLine($"{mi.Name}")

    //let mi = icty.GetMethod("CompareTo")
    //Console.WriteLine($"{mi}")

    //let x = 1
    //let y = 1
    //let z =
    //    mi.Invoke(x,[|y|])
    //    |> unbox<int>
    //Console.WriteLine($"z={z}")
    0
