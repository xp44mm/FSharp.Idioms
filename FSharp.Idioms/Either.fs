namespace FSharp.Idioms

/// 函数式编程中的标准模式
type Either<'Left, 'Right> =
    | Left of 'Left
    | Right of 'Right

/// 单参数版本，左右类型相同
type Either<'T> = Either<'T, 'T>

module Either =
    /// 获取值
    let value (e: Either<'T>) =
        match e with
        | Left v -> v
        | Right v -> v

    let tryLeft (e) =
        match e with
        | Left v -> Some v
        | Right _ -> None

    let tryRight (e) =
        match e with
        | Left _ -> None
        | Right v -> Some v
