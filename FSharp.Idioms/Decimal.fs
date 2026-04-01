module FSharp.Idioms.Decimal

open System

/// 将十进制字符转换为数值
let decimal_char_to_int c =
    match c with
    | _ when '0' <= c && c <= '9' -> int(c - '0') // int '0'
    | _ -> failwithf "Invalid decimal digit: %c" c

/// 从头尽可能取出数字字符，直到遇到非法字符。转换为整数，记录位数
let takeDigitsValueAndCount (buff: char list) =
    let rec loop chars (acc: int64) count =
        match chars with
        | c :: rest when '0' <= c && c <= '9' ->
            let value = int64(c - '0')
            loop rest (acc * 10L + value) (count + 1)
        | _ -> acc, count, chars
    loop buff 0L 0

/// 取出输入开头的数字，并转化为整数值，保留剩余字符
let takeDigits (buff: char list) =
    let value, count, rest = takeDigitsValueAndCount buff
    value, rest

/// 取出输入开头可能有的‘+’‘-’符号，并转化为整数值，保留剩余字符
let takeSign (chars: char list) =
    match chars with
    | '-' :: rest -> -1, rest
    | '+' :: rest -> 1, rest
    | _ -> 1, chars

/// 取有符号整数，输入开头可能有‘+’‘-’符号
let takeSInt (chars: char list) =
    let s, rest = takeSign chars
    let i, rest = takeDigits rest
    int64 s * i, rest

/// 从头解析符合json Number格式的数字: sign? digit+ fraction? exponent?，保留剩余字符
let takeNumber (buff: char list) =
    let takeDot (chars: char list) =
        match chars with
        | '.' :: rest -> takeDigitsValueAndCount rest
        | _ -> 0L, 0, chars

    let takeExponent (chars: char list) =
        match chars with
        | ('e' | 'E') :: rest ->
            let s, rest = takeSign rest
            let i, rest = takeDigits rest
            (s * int i), rest
        | _ -> 0, chars

    let s, rest = takeSign buff
    let i, rest = takeDigits rest
    let f, n, rest = takeDot rest
    let e, rest = takeExponent rest

    let d = i * pown 10L n + f
    let expo = e - n

    let value =
        if expo >= 0 then
            float s * float d * (10.0 ** float expo)
        else
            float s * float d / (10.0 ** float(-expo))
    value, rest

///如果输入是有符号整数，解析为整数值
let tryInt (str: string) =
    match str |> List.ofSeq |> takeSInt with
    | i, [] -> Some i
    | _ -> None

/// 如果输入是浮点数的格式，Json Number 解析为浮点数
let tryFloat (str: string) =
    match str |> List.ofSeq |> takeNumber with
    | i, [] -> Some i
    | _ -> None

///直接解析有符号整数，解析失败抛出异常
let parseInt (str: string) =
    match str |> List.ofSeq |> takeSInt with
    | i, [] -> i
    | _ -> FormatException str |> raise

/// 直接解析浮点数的格式，Json Number，解析失败抛出异常
let parseFloat (str: string) =
    match str |> List.ofSeq |> takeNumber with
    | i, [] -> i
    | _ -> FormatException str |> raise
