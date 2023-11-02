module FSharp.Idioms.Literals.Paren

open System
open System.Text.RegularExpressions
open FSharp.Reflection
open FSharp.Idioms

//
let typePrecedences =
    [
        []
        [":"]
        [","]
        ["->"]
        ["*"]
        ["<>";"[]"]
        ["max"]
    ]
    |> List.mapi(fun i ls ->
        ls
        |> List.map(fun sym -> sym,i*10)
    )
    |> List.concat
    |> Map.ofList


//优先级高的先计算
let valuePrecedences =
    [
        []
        [";"]
        [","]
        ["|||"]
        [" "]
        ["."]
        ["max"]
    ]
    |> List.mapi(fun i ls ->
        ls
        |> List.map(fun sym -> sym,i*10)
    )
    |> List.concat
    |> Map.ofList

//表达式不加括號環境優先級設爲0，必加括號環境優先級設爲一个肯定是最大的正整数
let putParen (precContext:int) (precExpr:int) (expr:string) =
    if precExpr > precContext then expr else "(" + expr + ")"
