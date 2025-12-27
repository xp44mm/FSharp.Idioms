namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.xUnit

type CharTest(output: ITestOutputHelper) =
    
    [<Fact>]
    member this.``isChineseChar - 基本汉字测试``() =
        // 基本汉字
        Should.equal true (Char.isChineseChar '我')
        Should.equal true (Char.isChineseChar '你')
        Should.equal true (Char.isChineseChar '好')
        Should.equal true (Char.isChineseChar '中')
        Should.equal true (Char.isChineseChar '国')
        Should.equal true (Char.isChineseChar '文')
        Should.equal true (Char.isChineseChar '字')
        
    [<Fact>]
    member this.``isChineseChar - CJK扩展A区汉字测试``() =
        // CJK扩展A区汉字
        Should.equal true (Char.isChineseChar '\u3400')  // 㐀
        Should.equal true (Char.isChineseChar '\u3401')  // 㐁
        Should.equal true (Char.isChineseChar '\u4DBF')  // 䶿
        
    [<Fact>]
    member this.``isChineseChar - 非汉字字符测试``() =
        // 英文字母
        Should.equal false (Char.isChineseChar 'A')
        Should.equal false (Char.isChineseChar 'z')
        Should.equal false (Char.isChineseChar '0')
        Should.equal false (Char.isChineseChar '9')
        
        // 标点符号
        Should.equal false (Char.isChineseChar '.')
        Should.equal false (Char.isChineseChar ',')
        Should.equal false (Char.isChineseChar '!')
        Should.equal false (Char.isChineseChar '?')
        
        // 空格
        Should.equal false (Char.isChineseChar ' ')
        Should.equal false (Char.isChineseChar '\t')
        
        // 其他语言字符
        Should.equal false (Char.isChineseChar 'あ')  // 日文平假名
        Should.equal false (Char.isChineseChar 'ア')  // 日文片假名
        Should.equal false (Char.isChineseChar '가')  // 韩文
        
    [<Fact>]
    member this.``isChineseChar - 边界测试``() =
        // 边界值测试
        Should.equal false (Char.isChineseChar (char 0x33FF))  // 基本汉字前一个
        Should.equal true  (Char.isChineseChar (char 0x3400))  // CJK扩展A区开始
        Should.equal true  (Char.isChineseChar (char 0x4DBF))  // CJK扩展A区结束
        Should.equal false (Char.isChineseChar (char 0x4DC0))  // CJK扩展A区后一个
        Should.equal false (Char.isChineseChar (char 0x4DFF))  // 基本汉字前一个
        Should.equal true  (Char.isChineseChar (char 0x4E00))  // 基本汉字开始
        Should.equal true  (Char.isChineseChar (char 0x9FFF))  // 基本汉字结束
        Should.equal false (Char.isChineseChar (char 0xA000))  // 基本汉字后一个
        
    [<Theory>]
    [<InlineData('我', true)>]
    [<InlineData('你', true)>]
    [<InlineData('好', true)>]
    [<InlineData('中', true)>]
    [<InlineData('A', false)>]
    [<InlineData('1', false)>]
    [<InlineData('.', false)>]
    [<InlineData(' ', false)>]
    [<InlineData('あ', false)>]  // 日文平假名
    [<InlineData('ア', false)>]  // 日文片假名
    [<InlineData('가', false)>]  // 韩文
    member this.``isChineseChar - 理论测试``(c: char, expected: bool) =
        let actual = Char.isChineseChar c
        Should.equal expected actual
        output.WriteLine($"字符 '{c}' (U+{(int c):X4}) 是汉字: {actual}, 预期: {expected}")
        
    [<Fact>]
    member this.``特殊字符测试``() =
        // 代理对测试（扩展B区汉字，需要两个char表示）
        // 注意：这里只能测试单个char的情况
        let surrogateHigh = char 0xD840  // 高代理
        let surrogateLow = char 0xDC00   // 低代理
        
        // 单个代理字符不是有效汉字
        Should.equal false (Char.isChineseChar surrogateHigh)
        Should.equal false (Char.isChineseChar surrogateLow)
                
        output.WriteLine($"代理对测试: 高代理=U+{(int surrogateHigh):X4}, 低代理=U+{(int surrogateLow):X4}")
