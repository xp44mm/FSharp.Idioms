module FSharp.Idioms.Hexadecimal

open System

// 将十六进制字符转换为数值
let hex_char_to_int c =
    match c with
    | _ when '0' <= c && c <= '9' -> int c - int '0'
    | _ when 'a' <= c && c <= 'f' -> int c - int 'a' + 10
    | _ when 'A' <= c && c <= 'F' -> int c - int 'A' + 10
    | _ -> failwithf "Invalid hex digit: %c" c

// 计算任意位数十六进制字符的码点
let getValue (hexChars: char list) =
    hexChars
    |> List.fold (fun acc c -> acc * 16 + hex_char_to_int c) 0

[<Obsolete("getValueN")>]
let getChar n buff = buff |> List.take n |> getValue |> char

// 从头取出 n 个数字转换成整数
let getValueN n (buff: char list) =
    let rec loop chars countDown acc =
        match chars with
        | _ when countDown = 0 -> acc, chars // 返回结果和剩余字符
        | [] -> acc, [] // 没有更多字符了
        | c :: rest -> loop rest (countDown - 1) (acc * 16 + hex_char_to_int c)
    loop buff n 0

// 从头尽可能取出数字字符转换为整数，直到遇到非法字符
let take (buff: char list) =
    let rec loop chars acc =
        match chars with
        | c :: rest when
            ('0' <= c && c <= '9')
            || ('a' <= c && c <= 'f')
            || ('A' <= c && c <= 'F')
            ->
            let value = hex_char_to_int c
            loop rest (acc * 16 + value)
        | _ -> acc, chars
    loop buff 0
