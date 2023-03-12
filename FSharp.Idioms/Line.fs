module FSharp.Idioms.Line

open System.Text.RegularExpressions
open ActivePatterns

/// 行位置，行，数量包括结尾的\n，行的内容。
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


/// 每行开始的空格数
let startSpaces lines =
    lines
    |> Seq.map(fun line -> Regex.Match(line,"^ *").Length)
    |> Seq.min

/// 各行同时缩进
/// spaces: 行首将要新增的空格个数
let indentCodeBlock (spaces:int) (codeBlock:string) =
    let spaces = StringOps.space spaces
    codeBlock
    |> splitLines
    |> Seq.map(fun (_,line) -> spaces+line)
    |> String.concat ""
