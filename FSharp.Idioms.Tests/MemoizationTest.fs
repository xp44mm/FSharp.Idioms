namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Literals
open FSharp.Idioms.Memoization

type MemoizationTest(output: ITestOutputHelper) =
    
    let rec fib n = if n <= 2 then 1 else fib (n - 1) + fib (n - 2)

    [<Fact>]
    member this.``decorate memoized``() =
        let fib myself n =
            output.WriteLine(sprintf "computing fib %d" n)
            if n <= 2 then 1 else myself (n - 1) + myself (n - 2)

        let fibonacci = memoize(fib)
        //注意：打印输出说明每个输入只计算一次
        let y3 = fibonacci 3
        output.WriteLine(sprintf  "fibonacci %d" y3)

        let y5 = fibonacci 5
        output.WriteLine(sprintf  "fibonacci %d" y5)

        let y5 = fibonacci 5
        output.WriteLine(sprintf  "fibonacci %d" y5)

        ()