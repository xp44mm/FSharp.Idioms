namespace FSharp.Idioms

open Xunit
open FSharp.Idioms.Literal
open System

type FormatValueTest(output: ITestOutputHelper) =
    
    // sbyte 测试
    [<Theory>]
    [<InlineData(0y, "G", "0")>]
    [<InlineData(-128y, "G", "-128")>]
    [<InlineData(127y, "G", "127")>]
    [<InlineData(42y, "D5", "00042")>]
    member this.``formatValue sbyte test``(value: sbyte, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // byte 测试
    [<Theory>]
    [<InlineData(0uy, "G", "0")>]
    [<InlineData(255uy, "G", "255")>]
    [<InlineData(42uy, "G", "42")>]
    [<InlineData(42uy, "D5", "00042")>]
    member this.``formatValue byte test``(value: byte, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // int16 测试
    [<Theory>]
    [<InlineData(123s, "G", "123")>]
    [<InlineData(-123s, "G", "-123")>]
    [<InlineData(123s, "D5", "00123")>]
    [<InlineData(0s, "G", "0")>]
    member this.``formatValue int16 test``(value: int16, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // int 测试
    [<Theory>]
    [<InlineData(123, "G", "123")>]
    [<InlineData(-123, "G", "-123")>]
    [<InlineData(123, "D5", "00123")>]
    [<InlineData(0, "G", "0")>]
    [<InlineData(123456, "N0", "123,456")>]
    member this.``formatValue int test``(value: int, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // int64 测试
    [<Theory>]
    [<InlineData(123456L, "G", "123456")>]
    [<InlineData(-123456L, "G", "-123456")>]
    [<InlineData(123456L, "D10", "0000123456")>]
    [<InlineData(0L, "G", "0")>]
    member this.``formatValue int64 test``(value: int64, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // single 测试
    [<Theory>]
    [<InlineData(123.456f, "G", "123.456")>]
    [<InlineData(-123.456f, "G", "-123.456")>]
    [<InlineData(123.456f, "F2", "123.46")>]
    [<InlineData(123.400f, "0.##", "123.4")>]
    [<InlineData(0.123f, "0.##", "0.12")>]
    [<InlineData(0.0f, "0.##", "0")>]
    member this.``formatValue single test``(value: single, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // double 测试
    [<Theory>]
    [<InlineData(123.456, "G", "123.456")>]
    [<InlineData(-123.456, "G", "-123.456")>]
    [<InlineData(123.456, "F2", "123.46")>]
    [<InlineData(123.456, "0.##", "123.46")>]
    [<InlineData(123.400, "0.##", "123.4")>]
    [<InlineData(123.000, "0.##", "123")>]
    [<InlineData(0.123, "0.##", "0.12")>]
    [<InlineData(0.100, "0.##", "0.1")>]
    [<InlineData(0.0, "0.##", "0")>]
    [<InlineData(-123.456, "0.##", "-123.46")>]
    [<InlineData(-0.123, "0.##", "-0.12")>]
    member this.``formatValue double test``(value: double, format: string, expected: string) =
        let result = formatValue format value
        Assert.Equal(expected, result)

    // bool 测试
    [<Theory>]
    [<InlineData(true, "true")>]
    [<InlineData(false, "false")>]
    member this.``formatValue bool test``(value: bool, expected: string) =
        let result = formatValue "G" value
        Assert.Equal(expected, result)

    // string 测试
    [<Theory>]
    [<InlineData("hello", "hello")>]
    [<InlineData("", "")>]
    [<InlineData("123", "123")>]
    [<InlineData("null", "null")>]
    [<InlineData("  spaces  ", "  spaces  ")>]
    member this.``formatValue string test``(value: string, expected: string) =
        let result = formatValue "G" value
        Assert.Equal(expected, result)

    // char 测试
    [<Theory>]
    [<InlineData('A', "A")>]
    [<InlineData('1', "1")>]
    [<InlineData(' ', " ")>]
    [<InlineData('\n', "\n")>]
    member this.``formatValue char test``(value: char, expected: string) =
        let result = formatValue "G" value
        Assert.Equal(expected, result)

    // DateTime 测试
    [<Theory>]
    [<InlineData(2026, 7, 26, 14, 30, 0, "yyyy-MM-dd HH:mm:ss", "2026-07-26 14:30:00")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, "yyyy-MM-dd", "2026-07-26")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, "HH:mm:ss", "14:30:00")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, "yyyy/MM/dd", "2026/07/26")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, "dd/MM/yyyy", "26/07/2026")>]
    member this.``formatValue datetime test``(year: int, month: int, day: int, hour: int, minute: int, second: int, format: string, expected: string) =
        let x = DateTime(year, month, day, hour, minute, second)
        let result = formatValue format x
        Assert.Equal(expected, result)

    // DateTime 默认格式测试
    [<Fact>]
    member this.``formatValue datetime with default format test``() =
        let x = DateTime(2026, 7, 26, 14, 30, 0)
        let result = formatValue null x
        Assert.Equal(x.ToString(), result)

    // DateTimeOffset 测试
    [<Theory>]
    [<InlineData(2026, 7, 26, 14, 30, 0, 0, "yyyy-MM-dd HH:mm:ss", "2026-07-26 14:30:00")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, 0, "yyyy-MM-dd", "2026-07-26")>]
    [<InlineData(2026, 7, 26, 14, 30, 0, 8, "yyyy-MM-dd HH:mm:ss", "2026-07-26 14:30:00")>]
    member this.``formatValue datetimeoffset test``(year: int, month: int, day: int, hour: int, minute: int, second: int, offsetHours: int, format: string, expected: string) =
        let x = DateTimeOffset(year, month, day, hour, minute, second, TimeSpan(offsetHours, 0, 0))
        let result = formatValue format x
        Assert.Equal(expected, result)

    // TimeSpan 测试
    [<Theory>]
    [<InlineData(1, 2, 3, 4, 5, "c", "1.02:03:04.0050000")>]
    [<InlineData(0, 0, 0, 0, 0, "c", "00:00:00")>]
    [<InlineData(0, 1, 2, 3, 4, "hh\\:mm\\:ss", "01:02:03")>]
    [<InlineData(0, 0, 0, 0, 0, "hh\\:mm\\:ss", "00:00:00")>]
    member this.``formatValue timespan test``(days: int, hours: int, minutes: int, seconds: int, milliseconds: int, format: string, expected: string) =
        let x = TimeSpan(days, hours, minutes, seconds, milliseconds)
        let result = formatValue format x
        Assert.Equal(expected, result)

    // null 测试
    [<Fact>]
    member this.``formatValue null test``() =
        let x = null
        let result = formatValue "G" x
        Assert.Equal("null", result)

    // 空格式字符串测试
    [<Theory>]
    [<InlineData(123)>]
    [<InlineData(123.456)>]
    [<InlineData("hello")>]
    [<InlineData('A')>]
    member this.``formatValue with empty format uses default ToString test``(value: obj) =
        let result = formatValue "" value
        Assert.Equal(value.ToString(), result)

    // null 格式字符串测试
    [<Theory>]
    [<InlineData(123)>]
    [<InlineData(123.456)>]
    [<InlineData("hello")>]
    [<InlineData('A')>]
    member this.``formatValue with null format uses default ToString test``(value: obj) =
        let result = formatValue null value
        Assert.Equal(value.ToString(), result)

    // 列表测试
    [<Fact>]
    member this.``formatValue list test``() =
        let x = [1; 2; 3]
        let result = formatValue "G" x
        Assert.Equal("[1;2;3]", result)

    [<Fact>]
    member this.``formatValue option none test``() =
        let x = None
        let result = formatValue "G" x
        Assert.Equal("None", result)

