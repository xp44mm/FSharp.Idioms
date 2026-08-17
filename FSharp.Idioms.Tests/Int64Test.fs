namespace FSharp.Idioms

open System
open Xunit

type Int64Test(output: ITestOutputHelper) =
    
    // ===== tryParse 测试 =====
    [<Theory>]
    [<InlineData("123", 123L)>]
    [<InlineData("-123", -123L)>]
    [<InlineData("0", 0L)>]
    [<InlineData("-5", -5L)>]
    [<InlineData("+123", 123L)>]
    [<InlineData("9223372036854775807", 9223372036854775807L)>]  // MaxValue
    [<InlineData("-9223372036854775808", -9223372036854775808L)>] // MinValue
    [<InlineData("1", 1L)>]
    [<InlineData("-1", -1L)>]
    [<InlineData("10", 10L)>]
    [<InlineData("-10", -10L)>]
    [<InlineData("0100", 100L)>]
    [<InlineData("-100", -100L)>]
    [<InlineData("2147483647", 2147483647L)>]  // Int32.MaxValue
    [<InlineData("-2147483648", -2147483648L)>] // Int32.MinValue
    member this.``tryParse - 有效整数测试``(str: string, expected: int64) =
        let result = Int64.tryParse str
        match result with
        | Some actual -> 
            Assert.Equal(expected, actual)
        | None -> 
            Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Theory>]
    [<InlineData(" 123 ")>]      // 有空格
    [<InlineData("abc")>]        // 字母
    [<InlineData("123.45")>]     // 小数
    [<InlineData("-123.45")>]    // 小数
    [<InlineData("1.2.3")>]      // 多个小数点
    [<InlineData("  ")>]         // 空白
    [<InlineData("")>]           // 空字符串
    [<InlineData("null")>]       // null
    [<InlineData("NaN")>]        // NaN
    [<InlineData("Infinity")>]   // Infinity
    [<InlineData(".")>]          // 只有小数点
    [<InlineData("-.5")>]        // 负数加小数点
    [<InlineData("+")>]          // 只有加号
    [<InlineData("-")>]          // 只有减号
    [<InlineData("123abc")>]     // 数字后跟字母
    [<InlineData("abc123")>]     // 字母后跟数字
    [<InlineData("--123")>]      // 多个负号
    [<InlineData("++123")>]      // 多个正号
    [<InlineData("+-123")>]      // 混合符号
    [<InlineData("12 3")>]       // 数字中有空格
    [<InlineData("12e3")>]       // 科学计数法
    [<InlineData("1_000")>]      // 下划线分隔符
    [<InlineData("0x123")>]      // 十六进制
    [<InlineData("0b1010")>]     // 二进制
    member this.``tryParse - 无效整数测试``(str: string) =
        let result = Int64.tryParse str
        Assert.Equal(None, result)
