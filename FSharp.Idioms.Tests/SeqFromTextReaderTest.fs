namespace FSharp.Idioms

open System.IO

open Xunit


open FSharp.xUnit
open FSharp.Idioms


type SeqFromTextReaderTest (output: ITestOutputHelper) =
    
    [<Fact>]
    member _. ``从空文本读取器返回空序列`` () =
        use reader = new StringReader("")
        let result = Seq.fromTextReader reader
        Assert.Empty(result)
    
    [<Fact>]
    member _. ``从单字符文本读取正确字符`` () =
        use reader = new StringReader("A")
        let result = Seq.fromTextReader reader |> Seq.toArray
        Should.equal [| 'A' |] result
    
    [<Fact>]
    member _. ``从多字符文本读取所有字符`` () =
        use reader = new StringReader("Hello World!")
        let result = Seq.fromTextReader reader |> Seq.toArray
        let expected = "Hello World!".ToCharArray()
        Should.equal expected result
    
    [<Fact>]
    member _. ``包含特殊字符和换行符`` () =
        let text = "Line1\nLine2\r\nTab\tTest"
        use reader = new StringReader(text)
        let result = Seq.fromTextReader reader |> Seq.toArray
        let expected = text.ToCharArray()
        Should.equal expected result
    
    [<Fact>]
    member _. ``支持Unicode字符`` () =
        let text = "中文🌍测试"
        use reader = new StringReader(text)
        let result = Seq.fromTextReader reader |> Seq.toArray
        let expected = text.ToCharArray()
        Should.equal expected result
    
    [<Fact>]
    member _. ``惰性求值 - 只在需要时读取`` () =
        use reader = new StringReader("ABCDEFG")
        let sequence = Seq.fromTextReader reader
        
        // 只取前3个字符，后面的不应该被读取（如果是惰性的）
        let firstThree = sequence |> Seq.take 3 |> Seq.toArray
        Should.equal [| 'A'; 'B'; 'C' |] firstThree
    
    [<Fact>]
    member _. ``多次迭代应该返回空序列（TextReader已到达末尾）`` () =
        use reader = new StringReader("Test")
        let sequence = Seq.fromTextReader reader
        
        // 第一次迭代应该成功
        let firstIteration = sequence |> Seq.toArray
        Should.equal [| 'T'; 'e'; 's'; 't' |] firstIteration
        
        // 第二次迭代返回空
        let arr2 = sequence |> Seq.toArray
        Assert.Empty(arr2)
            
    [<Fact>]
    member _. ``处理大文本文件`` () =
        // 创建一个大文本进行测试
        let largeText = String.replicate 1000 "ABCDEFGHIJKLMNOPQRSTUVWXYZ "
        use reader = new StringReader(largeText)
        let result = Seq.fromTextReader reader |> Seq.toArray
        let expected = largeText.ToCharArray()
        Should.equal expected result
        Should.equal 27000 result.Length // 26字母 + 空格 = 27字符 * 1000次
