module FSharp.Idioms.Char

// 判断单个字符是否是汉字
let isChineseChar (c: char) =
    let code = int c
    let a = code >= 0x4E00 && code <= 0x9FFF // 基本汉字范围：U+4E00 - U+9FFF
    let b = code >= 0x3400 && code <= 0x4DBF // CJK统一表意文字扩展：U+3400 - U+4DBF
    a || b
