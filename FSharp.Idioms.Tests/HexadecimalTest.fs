namespace FSharp.Idioms

open Xunit

open System
open FSharp.xUnit
open System.Text.RegularExpressions

type HexadecimalTest(output: ITestOutputHelper) =

    [<Theory>]
    [<Natural 6>]
    member this.``getValueN test``(i: int) =
        let cases =
            [
                2, [ '1'; 'A'; '3' ], 0x1A, [ '3' ] // 取2位十六进制
                3, [ 'F'; 'F'; 'F' ], 0xFFF, [] // 取3位十六进制
                1, [ '9'; 'A' ], 0x9, [ 'A' ] // 取1位十六进制
                4, [ '0'; '0'; '1'; '0' ], 0x10, [] // 取4位十六进制，含前导零
                2, [ 'A'; 'B'; 'C' ], 0xAB, [ 'C' ] // 取2位，剩余1位
                0, [ '1'; '2'; '3' ], 0, [ '1'; '2'; '3' ] // 取0位，返回0和原列表
            ]

        let n, input, expected, remaining = cases.[i]
        let actualValue, actualRemaining = Hexadecimal.getValueN n input
        Should.equal expected actualValue
        Should.equal remaining actualRemaining

    [<Theory>]
    [<Natural 7>]
    member this.``take test``(i: int) =
        let cases =
            [
                [ '1'; 'A'; '3'; 'G' ], 0x1A3, [ 'G' ] // 遇到非法字符G停止
                [ 'F'; 'F'; 'F' ], 0xFFF, [] // 全部合法字符
                [ '9'; 'A'; 'Z'; '1' ], 0x9A, [ 'Z'; '1' ] // 遇到Z停止
                [ '0'; '0'; '1' ], 0x1, [] // 含前导零
                [ 'C'; 'A'; 'F'; 'E' ], 0xCAFE, [] // 常见十六进制值
                [ 'X'; '1'; '2' ], 0, [ 'X'; '1'; '2' ] // 开头就是非法字符
                [], 0, [] // 空输入
            ]
        let input, expected, remaining = cases.[i]
        let actualValue, actualRemaining = Hexadecimal.take input
        Should.equal expected actualValue
        Should.equal remaining actualRemaining

    [<Theory>]
    [<Natural 6>]
    member this.``hex_char_to_int test``(i: int) =
        let cases =
            [
                '0', 0
                '9', 9
                'A', 10
                'F', 15
                'a', 10
                'f', 15
            ]
        let input, expected = cases.[i]
        let actual = Hexadecimal.hex_char_to_int input
        Should.equal expected actual

    [<Theory>]
    [<Natural 4>]
    member this.``getValue test``(i: int) =
        let cases =
            [
                [ '1'; 'A'; '3' ], 0x1A3
                [ 'F'; 'F' ], 0xFF
                [ '0'; '1' ], 0x1
                [ 'C'; 'A'; 'F'; 'E' ], 0xCAFE
            ]
        let input, expected = cases.[i]
        let actual = Hexadecimal.getValue input
        Should.equal expected actual
