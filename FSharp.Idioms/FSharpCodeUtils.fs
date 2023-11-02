module FSharp.Idioms.FSharpCodeUtils

open System.Text.RegularExpressions
open System.Globalization
open System

///是否為F#標識符
let isIdentifier (tok:string) =
    Regex.IsMatch(tok,@"^[\w-[\d]][\w']*$")

/// 如果需要，浮点数给整数加小数点
let decimalPoint (s:string) =
    if s.Contains "." || s.Contains "E" || s.Contains "e" then
        s
    else
        s + ".0"

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


let getGenericTypeName (type_name:string) =
    match type_name.Split('`').[0] with
    | "Void"         -> "unit"
    | "FSharpOption" -> "option"
    | "FSharpList"   -> "list"
    | "IEnumerable"  -> "seq"
    | "FSharpSet"    -> "Set"
    | "FSharpMap"    -> "Map"
    | "List"         -> "ResizeArray"
    | name -> name
