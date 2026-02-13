module FSharp.Idioms.Program

open System
open System.IO
open System.Text

open System
open System.Collections
open System.Collections.Generic

open FSharp.Idioms.Literal
open System
open System.Xml
open System.Reflection

open System
open System.IO
open System.Text

let folder = @"D:\Application Data\崔安琪\阅读与写作"

let merge () =

    let files = [
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之01引言.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之02诚实的自己的话.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之03源头.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之04组织.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之05文体.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之06叙述.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之07议论.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之08抒情.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之09描写.txt"
        @"D:\Application Data\崔安琪\怎样写作\201 作文论之10修辞.txt"
    ]

    let mergedContent =
        files
        |> List.map File.ReadAllText
        |> String.concat Environment.NewLine

    let outputFile = Path.Combine(Path.GetDirectoryName(files.[0]), "201 作文论.txt")

    File.WriteAllText(outputFile, mergedContent, Encoding.UTF8)

    Console.WriteLine($"文件已合并到: {outputFile}")
    Console.WriteLine($"文件大小: {FileInfo(outputFile).Length} 字节")

open System.Text.RegularExpressions

/// 更安全的文件名提取函数
let getSafeFileNameFromContent (firstLine: string) =
    // 移除#标记和首尾空白
    let title =
        if firstLine.StartsWith("# ") then
            firstLine.Substring(2).Trim()
        elif firstLine.StartsWith("#") then
            firstLine.Substring(1).Trim()
        else
            firstLine

    // 使用正则表达式移除非法字符
    let invalidChars =
        Regex.Escape(new string(Path.GetInvalidFileNameChars()))
    let invalidRegStr = sprintf "[%s]" invalidChars
    let cleanName = Regex.Replace(title, invalidRegStr, "_")

    // 限制文件名长度
    let maxLength = 100
    let truncatedName =
        if cleanName.Length > maxLength then
            cleanName.Substring(0, maxLength)
        else
            cleanName

    truncatedName

let splitFile () =
    let srcFile = Path.Combine(folder, "23456789.txt")
    let lines = File.ReadAllLines(srcFile, Encoding.UTF8)

    // 获取所有标题行（以"# "开头）
    let titleLines =
        lines
        |> Array.indexed
        |> Array.filter(fun (_, line) -> line.StartsWith("# "))
        |> Array.map(fun (i, line) -> (i, line.Trim()))

    // 打印标题行（验证）
    Console.WriteLine("找到的标题行:")
    for (lineNum, line) in titleLines do
        Console.WriteLine($"[{lineNum, 3}]{line}")

    let articles =
        lines
        |> Array.splitByIndex(titleLines |> Array.map fst)
        |> Array.map(fun (_, _, lines) -> lines)

    for lines in articles do
        let path =
            Path.Combine(folder, $"{getSafeFileNameFromContent lines.[0]}.txt")
        File.WriteAllLines(path, lines, Encoding.UTF8)

[<EntryPoint>]
let main _ =
    merge()
    0
