module FSharp.Idioms.UInt64

open System

/// 从头尽可能取出数字字符，直到遇到非法字符。转换为整数，记录位数
let takeValueAndBits (buff: char list) =
    let rec loop (rest: char list) (digits: uint64) (bits: int) =
        match rest with
        | c :: rest when '0' <= c && c <= '9' ->
            let value = uint64(c - '0')
            loop rest (digits * 10UL + value) (bits + 1)
        | _ -> digits, bits, rest
    loop buff 0UL 0
