module FSharp.Idioms.JsonPath

open System

open FSharp.Idioms.Jsons

let pluck (json: Json) (path: obj list) =
    let path = path |> List.map JsonKeyIndex.from

    let rec loop (json: Json) (path: JsonKeyIndex list) =
        match path with
        | [] -> Some json
        | key :: rest ->
            match json with
            | Json.Array elements when elements.Length > 0 ->
                match key with
                | ArrayIndex i ->
                    if 0 <= i && i < elements.Length then
                        loop json.[i] rest
                    else
                        None
                | _ -> None
            | Json.Object _ ->
                match key with
                | FieldKey name ->

                    if json.hasProperty name then
                        loop json.[name] rest
                    else
                        None
                | _ -> None
            | _ -> None

    loop json path

let enumerate (json: Json) : seq<obj list * Json> =
    let rec loop path value =
        seq {
            match value with
            | Json.Array elems ->
                for i = 0 to List.length elems - 1 do
                    yield! loop (ArrayIndex i :: path) elems.[i]

            | Json.Object fields ->
                for (key, value) in fields do
                    yield! loop (FieldKey key :: path) value

            | _ -> yield path, value
        }

    loop [] json
    |> Seq.map (fun (path, json) ->
        let path = path |> List.map (fun p -> p.toObject ()) |> List.rev
        path, json)

let update (json: Json) (chooser: obj list -> Json -> Json option) =
    let rec loop (path: obj list) (value: Json) =
        match chooser (List.rev path) value with
        | Some newjson -> newjson
        | None ->
            match value with
            | Json.Array elems -> elems |> List.mapi (fun i j -> loop (box i :: path) j) |> Json.Array
            | Json.Object fields -> fields |> List.map (fun (k, j) -> k, loop (box k :: path) j) |> Json.Object

            | _ -> value

    loop [] json
