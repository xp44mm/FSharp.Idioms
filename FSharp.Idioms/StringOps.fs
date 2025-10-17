module FSharp.Idioms.StringOps

open System
open System.Text.RegularExpressions

/////忽略大小写的方法比较字符串
//[<Obsolete("OrdinalIgnoreCase.(==)")>]
//let (==) a b = StringComparer.OrdinalIgnoreCase.Equals(a,b)
//[<Obsolete("OrdinalIgnoreCase.(!=)")>]
//let (!=) a b = OrdinalIgnoreCase.(!=) a b


//[<Obsolete("Line.space")>]
//let space i = " " ** i

//[<Obsolete("Line.space4")>]
//let space4 i = " " ** (4*i)

//[<Obsolete("Line.space4")>]
//let indent i = " " ** (4*i)


//[<Obsolete(nameof tryStartsWith)>]
//let tryStart (prefix:string) (inp:string) =
//    tryStartsWith prefix inp
//    |> Option.map(fun _ ->
//        inp.[prefix.Length..]
//    )


    
//[<Obsolete(nameof RegularExpressions.trySearch)>]
//let tryRegexMatch (re: Regex) (input:string) = 
//    RegularExpressions.trySearch re input
//    |> Option.map(fun m ->
//        m.Value,input.[m.Index+m.Value.Length..]
//    )

//[<Obsolete(nameof RegularExpressions.trySearch)>]
//let tryMatch (re: Regex) (input:string) = 
//    RegularExpressions.trySearch re input
//    |> Option.map(fun m ->
//        m.Value,input.[m.Index+m.Value.Length..]
//    )

//[<Obsolete(nameof tryStartsWith)>]
//let tryStartWith = tryStartsWith

///// 匹配前缀，用正则表达式的模式
//[<Obsolete(nameof RegularExpressions.trySearch)>]
//let tryPrefix (pattern:string) =
//    let re = Regex (String.Format("^(?:{0})", pattern))
//    fun inp ->
//        RegularExpressions.trySearch re inp
//        |> Option.map(fun m ->
//            m.Value,inp.[m.Index+m.Value.Length..]
//        )

/////输入字符串的前缀子字符串符合给定的模式
//[<Obsolete(nameof RegularExpressions.(|Rgx|_|))>]
//let (|Prefix|_|) (pattern:string) =
//    let re = Regex (String.Format("^(?:{0})", pattern))
//    fun inp ->
//        RegularExpressions.trySearch re inp
//        |> Option.map(fun m ->
//            m.Value,inp.[m.Index+m.Value.Length..]
//        )

/////匹配输入字符串的第一个字符，返回剩余字符串
//[<Obsolete("(|First|_)")>]
//let (|PrefixChar|_|) = tryFirst

