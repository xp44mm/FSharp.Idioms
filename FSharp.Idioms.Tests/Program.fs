module FSharp.Idioms.Program

open System
open System.IO
open System.Text

open System
open System.Collections
open System.Collections.Generic

open FSharp.Idioms.Literals
open System
open System.Xml
open System.Reflection

let [<EntryPoint>] main _ =
    let text = File.ReadAllText("D:\excel.tsv",Encoding.UTF8)
    Console.Write(Literal.stringify text)
    0
