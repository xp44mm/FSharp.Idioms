module FSharp.Idioms.ComparisonActivePatterns

let inline (|GE|_|) a x = if x >= a then Some() else None
let inline (|GT|_|) a x = if x > a  then Some() else None
let inline (|LE|_|) a x = if x <= a then Some() else None
let inline (|LT|_|) a x = if x < a  then Some() else None
let inline (|NE|_|) a x = if x <> a then Some() else None
let inline (|EQ|_|) a x = if x = a  then Some() else None
