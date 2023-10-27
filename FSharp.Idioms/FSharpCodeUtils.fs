module FSharp.Idioms.FSharpCodeUtils

open System.Text.RegularExpressions
open System.Globalization
open System

///是否為F#標識符
let isIdentifier (tok:string) =
    Regex.IsMatch(tok,@"^[\w-[\d]][\w']*$")

let unescapeChar c =     
    match c with
    | '\\' -> @"\\"
    | '\a' -> @"\a"
    | '\b' -> @"\b"
    | '\t' -> @"\t"
    | '\n' -> @"\n"
    | '\v' -> @"\v"
    | '\f' -> @"\f"
    | '\r' -> @"\r"
    | '\u007F' -> @"\u007F"
    | c when c < '\u0010' -> @"\u000" + Convert.ToString(Convert.ToInt16(c),16).ToUpper()
    | c when c < '\u0020' -> @"\u00" + Convert.ToString(Convert.ToInt16(c),16).ToUpper()
    | c -> c.ToString(CultureInfo.InvariantCulture)

/// xyz -> "xyz"
let toStringLiteral (value:string) =
    value.ToCharArray()
    |> Array.map(fun c ->
        if c = '\"' then "\\\"" else unescapeChar c
    )
    |> String.concat ""
    |> sprintf "\"%s\""

/// c -> 'c'
let toCharLiteral c = 
    if c = '\'' then @"\'" else unescapeChar c
    |> sprintf "'%s'"

//表达式不加括號環境優先級設爲0，必加括號環境優先級設爲一个肯定是最大的数字
let putparen (precContext:int) (precExpr:int) (expr:string) =
    if precExpr > precContext then expr else "(" + expr + ")"
