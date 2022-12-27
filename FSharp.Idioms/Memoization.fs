module FSharp.Idioms.Memoization

open System.Collections.Generic
open System.Collections.Concurrent

let memoize (f : ('T -> 'U) -> 'T -> 'U) =
    let d = new ConcurrentDictionary<'T, 'U>(HashIdentity.Structural)
    let rec g x = d.GetOrAdd(x, f g)
    g

let memoized (f : ('T -> 'U) -> 'T -> 'U) =
    let d = new Dictionary<'T, 'U>(HashIdentity.Structural)
    let rec g x =
        match d.TryGetValue x with
        | true, res -> res
        | _ -> 
            let res = f g x 
            d.Add(x, res)
            res
    g
