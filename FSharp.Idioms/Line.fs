module FSharp.Idioms.Line

open System.Text.RegularExpressions

/// 行位置，行，数量包括结尾的\n，行的内容。
let splitLines(text:string) =
    let rec loop pos (inp:string) =
        seq{
            match inp with
            | "" -> ()
            | On (tryPrefix @"[^\r\n]*(\r?\n|\r)") (line, rest) ->
                /// pos: 行首的位置
                /// line: 行的内容
                yield pos, line
                yield! loop (pos+line.Length) rest

            | _ -> yield pos,inp //inp剩下最后一行了

        }
    loop 0 text

///绝对位置计算行列索引值
let rowColumn lines (pos:int) =
    let row,col =
        lines
        |> Seq.mapi(fun r (p,l)-> r,p,l)
        |> Seq.pick(fun(row,lpos,line:string)-> 
            if lpos <= pos && pos < lpos + line.Length then
                let col = pos - lpos
                Some(row,col)
            else None
            )
    row,col

/// 每行开始的空格数
let startSpaces lines =
    lines
    |> Seq.map(fun line -> Regex.Match(line,"^ *").Length)
    |> Seq.min

/// 各行同时缩进
let indentCodeBlock (spaces:int) (codeBlock:string) =
    let spaces = space spaces
    codeBlock
    |> splitLines
    |> Seq.map(fun (_,line) -> spaces+line)
    |> String.concat ""