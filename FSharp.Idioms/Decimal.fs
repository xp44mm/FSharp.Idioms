module FSharp.Idioms.Decimal

open System

/// 将十进制字符转换为数值
let decimal_char_to_int c =
    match c with
    | _ when '0' <= c && c <= '9' -> int c - 48 // int '0'
    | _ -> failwithf "Invalid decimal digit: %c" c

/// 从头尽可能取出数字字符，直到遇到非法字符。转换为整数，记录位数
let takeDigitsValueAndCount (buff: char list) =
    let rec loop chars (acc: int64) count =
        match chars with
        | c :: rest when '0' <= c && c <= '9' ->
            let value = int64(decimal_char_to_int c)
            loop rest (acc * 10L + value) (count + 1)
        | _ -> acc, count, chars
    loop buff 0L 0

let takeDigits (buff: char list) =
    let value, count, rest = takeDigitsValueAndCount buff
    value, rest

let takeSign (chars: char list) =
    match chars with
    | '-' :: rest -> -1, rest
    | '+' :: rest -> 1, rest
    | _ -> 1, chars

/// 取有符号整数
let takeSInt (chars: char list) =
    let s, rest = takeSign chars
    let i, rest = takeDigits rest
    int64 s * i, rest

/// 完整数字: sign? digit+ fraction? exponent?
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
