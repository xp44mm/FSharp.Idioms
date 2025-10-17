module FSharp.Idioms.Line

open System
open ActivePatterns

let ident space (code: string) = String.replicate space " " + code

let splitToLines (text: string) =
    text.Split([| "\r\n"; "\r"; "\n" |], StringSplitOptions.None)

/// 缩进输入块的每一行，并合并为一个字符串
let identAll space (blocks: string seq) = 
    blocks
    |> Seq.collect(fun blck -> splitToLines blck)
    |> Seq.map(ident space)
    |> String.concat "\r\n"

/// 每行开始的空格数
let startSpaces (lines: string seq) =
    lines
    |> Seq.map (fun line ->
        line
        |> Seq.takeWhile ((=) ' ')
        |> Seq.length)
    |> Seq.min

[<Obsolete("ident i s")>]
let space i (s:string) = ident i s

[<Obsolete("ident (4*i) s")>]
let space4 i (s:string) = ident (4*i) s

/// 各行同时缩进
/// spaces: 行首将要新增的空格个数
[<Obsolete("identAll blocks")>]
let indentCodeBlock (spaces:int) (codeBlock:string) =
    codeBlock
    |> splitToLines
    |> Seq.map(fun line -> ident spaces line)
    |> String.concat "\r\n"

/// 行位置，行，数量包括结尾的\n，行的内容。
[<Obsolete("splitToLines text")>]
let splitLines(text:string) =
    // pos: 行首的位置
    let rec loop pos (inp:string) =
        seq{
            match inp with
            | "" -> ()
            | Rgx @"^.*(\r?\n|\r)" m ->
                // line: 行的内容
                let line = m.Value
                let rest = inp.Substring(line.Length)
                yield pos, line
                yield! loop (pos+line.Length) rest

            | _ -> yield pos,inp //inp剩下最后一行了

        }
    loop 0 text

///绝对位置计算行列索引值 coordinate
let rowColumn (lines:seq<int*string>) (pos:int) =
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

/// 计算一个位置pos是第几列；以及pos下一行的开始位置。
/// lpos，行开始位置，linp从lpos以后的剩余文本 = inp.[lpos..]，pos某个字符在总文本中的位置
let getColumnAndLpos (lpos:int, linp:string) (pos:int) =
    let rec loop li =
        match linp.[li-lpos..] with
        | "" -> 
            failwith $"should length:{li} < pos:{pos}"
        | Rgx @"^[^\n]*\n" m ->
            let nextLpos = li + m.Length
            if pos < nextLpos then
                let col = pos - li
                col, nextLpos
            else
                loop nextLpos
        | linp ->
            let nextLpos = li + linp.Length
            if pos < nextLpos then
                let col = pos - li
                col,nextLpos
            else
                failwithf "eof:%d < pos:%d" nextLpos pos
    //fst:pos对应的列数，snd:pos的下一行开始位置。
    loop lpos

