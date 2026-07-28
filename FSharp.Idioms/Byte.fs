module FSharp.Idioms.Byte

/// 将十进制字符转换为数值
let fromChar c =
    match c with
    | _ when '0' <= c && c <= '9' -> byte (c - '0') // int '0'
    | _ -> failwithf "Invalid decimal digit: %c" c
