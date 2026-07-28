module FSharp.Idioms.JsonNumber

open System

type private ExponentNumber =
    | Exponent of int
    | OnlyE

    static member parse(chars: char list) =
        match chars with
        | [ 'e' | 'E' ] -> Some OnlyE, []
        | ('e' | 'E') :: rest ->
            let sign, rest = Char.tryPlusMinusRest rest
            let digits, bits, rest = UInt64.takeValueAndBits rest
            if bits = 0 then
                None, chars
            else
                let sign = defaultArg sign 1
                let e = sign * int digits
                Some(Exponent e), rest
        | _ -> None, chars

type private RealNumber =
    | Integer of sign: int option * integer: uint64
    | DotEnded of sign: int option * integer: uint64
    | DotNumber of sign: int option * integer: uint64 * places: int * fraction: uint64

    static member parse(chars: char list) =
        let sign, restAfterSign = Char.tryPlusMinusRest chars
        let integer, intCount, restAfterInt =
            UInt64.takeValueAndBits restAfterSign
        if intCount = 0 then
            None
        else
            match restAfterInt with
            | '.' :: restAfterDot ->
                let fraction, fracCount, restAfterFrac =
                    UInt64.takeValueAndBits restAfterDot
                if fracCount > 0 then
                    Some(DotNumber(sign, integer, fracCount, fraction), restAfterFrac)
                else
                    Some(DotEnded(sign, integer), restAfterDot)
            | _ -> Some(Integer(sign, integer), restAfterInt)

    /// 获取有效数字（保留原始小数位数信息）
    member this.significantDigits() =
        match this with
        | Integer(sign, i) -> sign, i, 0
        | DotEnded(sign, i) -> sign, i, 0
        | DotNumber(sign, i, places, f) -> sign, i * pown 10UL places + f, places

    /// 转换为浮点数
    member this.toFloat() =
        let sign, f =
            match this with
            | Integer(sign, i) -> sign, float i
            | DotEnded(sign, i) -> sign, float i
            | DotNumber(sign, i, places, f) -> sign, float i + float f / pown 10.0 places
        let sign = defaultArg sign 1
        float sign * f

let take (chars: char list) =
    if List.isEmpty chars then
        None, chars
    else
        match RealNumber.parse chars with
        | None -> None, chars
        | Some(realNum, restAfterReal) ->
            match ExponentNumber.parse restAfterReal with
            | (None | Some OnlyE), _ -> Some(realNum.toFloat()), restAfterReal
            | Some(Exponent expNum), restAfterExp ->
                let sign, integer, places = realNum.significantDigits()
                let sign = defaultArg sign 1

                let diff = expNum - places

                let realValue = float sign * float integer
                let value =
                    if diff >= 0 then
                        realValue * pown 10.0 diff
                    else
                        realValue / pown 10.0 (-diff)
                Some value, restAfterExp

///  Try to parse a string as a JSON number. Returns Some(float) if successful, or None if the input is not a valid JSON number.
let tryParse (inp: string) =
    if String.IsNullOrWhiteSpace inp then
        None
    else
        let chars = List.ofSeq inp
        match take chars with
        | Some value, [] -> Some value
        | _ -> None
