namespace FSharp.Idioms

open Xunit


open FSharp.Idioms.Jsons
open FSharp.xUnit
open FSharp.Idioms.Literal
open System

type FormatValueTest(output: ITestOutputHelper) =
    
    // 测试基本类型
    [<Fact>]
    member this.``formatValue sbyte test``() =
        let x = 0y
        let result = formatValue x "G"
        Assert.Equal("0", result)
    
    [<Fact>]
    member this.``formatValue byte test``() =
        let x = 42uy
        let result = formatValue x "G"
        Assert.Equal("42", result)
    
    [<Fact>]
    member this.``formatValue short test``() =
        let x = 123s
        let result = formatValue x "G"
        Assert.Equal("123", result)
    
    [<Fact>]
    member this.``formatValue int test``() =
        let x = 123
        let result = formatValue x "G"
        Assert.Equal("123", result)
    
    [<Fact>]
    member this.``formatValue int with format D5 test``() =
        let x = 123
        let result = formatValue x "D5"
        Assert.Equal("00123", result)
    
    [<Fact>]
    member this.``formatValue long test``() =
        let x = 123456L
        let result = formatValue x "G"
        Assert.Equal("123456", result)
    
    //[<Fact>]
    //member this.``formatValue float test``() =
    //    let x = 123.456
    //    let result = formatValue x "G"
    //    Assert.Equal("123.456", result)
    
    //[<Fact>]
    //member this.``formatValue float with format F2 test``() =
    //    let x = 123.456
    //    let result = formatValue x "F2"
    //    Assert.Equal("123.46", result)
    
    //[<Fact>]
    //member this.``formatValue float with custom format test``() =
    //    let x = 123.456
    //    let result = formatValue x "0.##"
    //    Assert.Equal("123.46", result)
    
    [<Fact>]
    member this.``formatValue decimal test``() =
        let x = 123.456m
        let result = formatValue x "G"
        Assert.Equal("123.456", result)
    
    [<Fact>]
    member this.``formatValue decimal with format F2 test``() =
        let x = 123.456m
        let result = formatValue x "F2"
        Assert.Equal("123.46", result)
    
    [<Fact>]
    member this.``formatValue single test``() =
        let x = 123.456f
        let result = formatValue x "G"
        Assert.Equal("123.456", result)
    
    [<Fact>]
    member this.``formatValue string test``() =
        let x = "hello"
        let result = formatValue x "G"
        Assert.Equal("hello", result)
    
    [<Fact>]
    member this.``formatValue null test``() =
        let x = null
        let result = formatValue x "G"
        Assert.Equal("<null>", result)
    
    [<Fact>]
    member this.``formatValue datetime test``() =
        let x = DateTime(2026, 7, 26, 14, 30, 0)
        let result = formatValue x "yyyy-MM-dd HH:mm:ss"
        Assert.Equal("2026-07-26 14:30:00", result)
    
    [<Fact>]
    member this.``formatValue datetime with default format test``() =
        let x = DateTime(2026, 7, 26, 14, 30, 0)
        let result = formatValue x null
        // 使用 ToString() 的默认格式
        Assert.Equal(x.ToString(), result)
    
    [<Fact>]
    member this.``formatValue datetimeoffset test``() =
        let x = DateTimeOffset(2026, 7, 26, 14, 30, 0, TimeSpan.Zero)
        let result = formatValue x "yyyy-MM-dd HH:mm:ss"
        Assert.Equal("2026-07-26 14:30:00", result)
    
    [<Fact>]
    member this.``formatValue timespan test``() =
        let x = TimeSpan(1, 2, 3, 4, 5)
        let result = formatValue x "c"
        Assert.Equal("1.02:03:04.0050000", result)
    
    [<Fact>]
    member this.``formatValue char test``() =
        let x = 'A'
        let result = formatValue x "G"
        Assert.Equal("A", result)
    
    // 测试空格式字符串
    [<Fact>]
    member this.``formatValue with empty format uses default ToString test``() =
        let x = 123
        let result = formatValue x ""
        Assert.Equal("123", result)
    
    [<Fact>]
    member this.``formatValue with null format uses default ToString test``() =
        let x = 123.456
        let result = formatValue x null
        Assert.Equal("123.456", result)
    
    // 测试复杂类型（使用 stringify 作为备用）
    [<Fact>]
    member this.``formatValue custom type test``() =
        let x = (1, "hello", 2)
        let result = formatValue x "G"
        Assert.Equal("""1,"hello",2""", result)

    [<Theory>]
    [<InlineData(123.456, "F2", "123.46")>]
    [<InlineData(123.456, "0.##", "123.46")>]
    [<InlineData(123.400, "0.##", "123.4")>]
    [<InlineData(123.000, "0.##", "123")>]
    [<InlineData(0.123, "0.##", "0.12")>]
    [<InlineData(0.100, "0.##", "0.1")>]
    [<InlineData(0.0, "0.##", "0")>]
    member this.``formatValue float with custom format test``(value: double, format: string, expected: string) =
        let result = formatValue value format
        Assert.Equal(expected, result)
    
    [<Theory>]
    [<InlineData(true, "true")>]
    [<InlineData(false, "false")>]
    member this.``formatValue bool test``(value: bool, expected: string) =
        let result = formatValue value "G"
        Assert.Equal(expected, result)
    
    [<Theory>]
    [<InlineData("hello", "hello")>]
    [<InlineData("", "")>]
    [<InlineData("123", "123")>]
    member this.``formatValue string test``(value: string, expected: string) =
        let result = formatValue value "G"
        Assert.Equal(expected, result)
        
    [<Fact>]
    member this.``formatValue with empty format test``() =
        let x = 123
        let result = formatValue x ""
        Assert.Equal("123", result)
    
    [<Fact>]
    member this.``formatValue with null format test``() =
        let x = 123.456
        let result = formatValue x null
        Assert.Equal("123.456", result)
