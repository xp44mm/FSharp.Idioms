namespace FSharp.Idioms.Literals

open Xunit
open Xunit.Abstractions

open System

/// 结论：双精度数圆整到14位，小数点14位之后的小数随机变化。
type IEqualityComparerTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``float equals``() =
        let x = 4./3.
        let y = 1.3333333333333333 // 16个3
        Assert.Equal(x,y) //小数点后16位相等

    [<Fact>]
    member this.``float 打印值圆整``() =
        //打印值会圆整，误差将有负数
        let x = -1.33333333333333333 // 17个3
        Assert.Equal(x.ToString(),"-1.3333333333333333") //16个3

        let y = -2.6666666666666666 //16个6
        Assert.Equal(y.ToString(),"-2.6666666666666665") //15个6，1个5

        let z = -3.999999999999999 //15个9                
        Assert.Equal(z.ToString(),"-3.999999999999999") //15个9

    [<Fact>]
    member this.``float round-trip``() =
        let x = 4./3.

        let s = x.ToString("R",Globalization.CultureInfo.InvariantCulture)
        //output.WriteLine(s)

        let len = s.Length-1
        Assert.Equal(len,17)

        let y = Double.Parse s
        Assert.Equal(x,y)
