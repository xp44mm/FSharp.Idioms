namespace FSharp.Idioms

open System
open Xunit

type JsonNumberTest(output: ITestOutputHelper) =
    
    // ===== tryParse 测试 =====
    [<Theory>]
    [<InlineData("123.05", 123.05)>]
    [<InlineData("-123.45", -123.45)>]
    [<InlineData("0", 0.0)>]
    [<InlineData("0.0", 0.0)>]
    [<InlineData("-5", -5.0)>]
    [<InlineData("+123", 123.0)>]
    [<InlineData("1.23e5", 123000.0)>]
    [<InlineData("1.23E-5", 0.0000123)>]
    [<InlineData("-1.23e+5", -123000.0)>]
    [<InlineData("123e-3", 0.123)>]
    [<InlineData("0.001", 0.001)>]
    [<InlineData("1000.0", 1000.0)>]
    [<InlineData("1e0", 1.0)>]
    [<InlineData("1E+0", 1.0)>]
    member this.``tryParse - 有效数字测试``(str: string, expected: float) =
        let result = JsonNumber.tryParse str
        match result with
        | Some actual -> 
            let diff = Math.Abs(actual - expected)
            Assert.True(diff < 0.0001, $"期望 {expected}，实际 {actual}，差异 {diff}")
        | None -> 
            Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Theory>]
    [<InlineData(" 123 ")>]
    [<InlineData(".5")>]
    [<InlineData("abc")>]
    [<InlineData("123.45.67")>]
    [<InlineData("1.2.3")>]
    [<InlineData("  ")>]
    [<InlineData("")>]
    [<InlineData("null")>]
    [<InlineData("NaN")>]
    [<InlineData("Infinity")>]
    [<InlineData("12e")>]
    [<InlineData("e10")>]
    [<InlineData("1e+")>]
    [<InlineData("1.2e")>]
    [<InlineData("1e.5")>]
    [<InlineData(".")>]
    [<InlineData("-.5")>]
    [<InlineData("+")>]
    [<InlineData("-")>]
    [<InlineData("1..2")>]
    [<InlineData("123abc")>]
    [<InlineData("abc123")>]
    member this.``tryParse - 无效数字测试``(str: string) =
        let result = JsonNumber.tryParse str
        Assert.Equal(None, result)
