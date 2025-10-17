module FSharp.Idioms.Pair

let swap (x,y) = y,x

let ofApp x y = x,y

let revApp x y = y,x // ofApp >> swap

///
let append x y = y,x

let prepend x y = x,y
