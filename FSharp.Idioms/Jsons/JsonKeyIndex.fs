namespace FSharp.Idioms.Jsons

type JsonKeyIndex =
    | ArrayIndex of int
    | FieldKey of string

    static member from(obj: obj) =
        match obj with
        | :? int as i -> ArrayIndex i
        | :? string as k -> FieldKey k
        | _ -> failwith "never"

    member this.toObject() : obj =
        match this with
        | ArrayIndex i -> box i
        | FieldKey k -> box k
