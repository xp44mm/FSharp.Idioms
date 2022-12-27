namespace FSharp.Idioms

///三元组(x,y,z)
module Triple =
    /// 第一分量
    let first (x,y,z) = x
    /// 前两分量
    let firstTwo (x,y,z) = x,y
    /// 最后分量
    let last (x,y,z) = z
    /// 最后两分量
    let lastTwo (x,y,z) = y,z
    /// 端点
    let ends (x,y,z) = x,z
    /// 中点
    let middle (x,y,z) = y
    
    let ofApp x y z = x,y,z
