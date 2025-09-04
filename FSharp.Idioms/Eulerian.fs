module FSharp.Idioms.Eulerian

type Vector = {
    startPoint: int * int
    endPoint: int * int
    penUp: bool
} with

    static member just(startp: int * int, endp: int * int, penUp: bool) = {
        startPoint = startp
        endPoint = endp
        penUp = penUp
    }

    member vector.rev() = { vector with startPoint = vector.endPoint; endPoint = vector.startPoint }

and Line = {
    points: Set<int * int>
    penUp: bool
} with

    static member create(points: Set<int * int>, penUp: bool) =
        if points.Count <> 2 then
            failwith "line has 2 points"

        { points = points; penUp = penUp }

    member line.toVectors() =
        let points = line.points |> Set.toArray
        let p1 = points.[0]
        let p2 = points.[1]
        [ Vector.just (p1, p2, line.penUp); Vector.just (p2, p1, line.penUp) ]

let getLineSet (lines: list<Line>) =
    let st = lines |> Set.ofList
    if lines.Length > st.Count then failwith "不能重复" else st

let forwardNextVector (vectors: Vector Set) (point: int * int) =
    vectors
    |> Seq.tryFind (fun v -> v.startPoint = point)
    |> Option.map (fun vec ->
        let rest = vectors |> Set.remove vec |> Set.remove (vec.rev ())
        vec, rest
    )

let lastPoint (revVectors: Vector list) = revVectors.Head.endPoint
let firstPoint (vectors: Vector list) = vectors.Head.startPoint

let rec basicSort (sortedVectors: Vector list) (restVectors: Vector Set) =
    match sortedVectors |> lastPoint |> forwardNextVector restVectors with
    | None -> List.rev sortedVectors, restVectors
    | Some(vec, rest) -> basicSort (vec :: sortedVectors) rest

let rec insertRing (skip: Vector list) (basePath: Vector list) (restVectors: Vector Set) =
    match basePath with
    | [] -> if restVectors.IsEmpty then List.rev skip else failwith "输入线条不是一笔连"

    | vec :: restPath ->
        let pnt = vec.startPoint // from endPoint

        match forwardNextVector restVectors pnt with
        | None -> insertRing (vec :: skip) restPath restVectors

        | Some(ringStart, remainingVectors) ->
            let ringPath, remainingAfterRing = basicSort [ ringStart ] remainingVectors
            if (List.last ringPath).endPoint <> pnt then
                failwith $"找到的环不闭合{ringPath}"
            let newPath = ringPath @ basePath
            insertRing skip newPath remainingAfterRing

///
let sortVectors (lines: list<Line>) =
    let vectors = set [
        for line in getLineSet lines do
            yield! line.toVectors ()
    ]

    match forwardNextVector vectors (0, 0) with
    | None -> failwith $"输入应该开始于原点"
    | Some(startVector, restVectors) ->
        let sorted, rest = basicSort [ startVector ] restVectors
        if rest.IsEmpty then sorted else insertRing [] sorted rest
