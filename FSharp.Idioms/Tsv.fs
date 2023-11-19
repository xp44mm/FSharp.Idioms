/// Tab-Separated Values
module  FSharp.Idioms.Tsv

open System
open System.IO
open System.Text
open System.Text.RegularExpressions

/// get cells
let parseTsv path =
    let lines = File.ReadAllLines(path,Encoding.UTF8)
    lines
    |> Array.map(fun ln -> ln.Split [|'\t'|])

/// get cells
let parseText (text:string) =
    let text = Regex.Replace(text,@"(\r\n|\r|\n)$","")
    let lines = Regex.Split(text,@"\r\n|\r|\n")
    lines
    |> Array.map(fun ln -> ln.Split [|'\t'|])

/// 合成tsv格式的文本
let stringify (cells:string[][]) =
    cells
    |> Array.map(fun cols ->
        cols |> String.concat "\t"
    )
    |> String.concat "\r\n"

/// 第一行是标题行
let getFieldTitles (rows:string[][]) =
    rows.[0]
    |> Array.mapi(fun i s -> s,i)
    |> Map.ofArray

/// get string value
let getValue (titles:Map<string,int>) title (row:string[]) = 
    row.[titles.[title]]

let getFloatValue (titles:Map<string,int>) title (row:string[]) =
    Double.Parse(getValue titles title row)

let getIntValue (titles:Map<string,int>) title (row:string[]) =
    getValue titles title row
    |> Int32.Parse

