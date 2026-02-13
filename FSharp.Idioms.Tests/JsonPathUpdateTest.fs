namespace FSharp.Idioms

open Xunit


open FSharp.Idioms.Jsons
open FSharp.xUnit

type JsonPathUpdateTest(output: ITestOutputHelper) =
    
    [<Fact>]
    member _.``update primitive value with matching path``() =
        let json = Json.String "old"
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [] -> Some (Json.String "new")
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.String "new"
        Should.equal expected result

    [<Fact>]
    member _.``update primitive value with non-matching path``() =
        let json = Json.Number 42.0
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "invalid"] -> Some (Json.Number 100.0)
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Number 42.0
        Should.equal expected result

    [<Fact>]
    member _.``update array element``() =
        let json = Json.Array [Json.String "a"; Json.String "b"; Json.String "c"]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [ArrayIndex 1] -> Some (Json.String "updated")
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Array [Json.String "a"; Json.String "updated"; Json.String "c"]
        Should.equal expected result

    [<Fact>]
    member _.``update nested array element``() =
        let json = Json.Array [
            Json.Array [Json.String "a"; Json.String "b"]
            Json.String "c"
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [ArrayIndex 0; ArrayIndex 1] -> Some (Json.String "updated")
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Array [
            Json.Array [Json.String "a"; Json.String "updated"]
            Json.String "c"
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update object field``() =
        let json = Json.Object [
            "name", Json.String "John"
            "age", Json.Number 30.0
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "age"] -> Some (Json.Number 31.0)
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "name", Json.String "John"
            "age", Json.Number 31.0
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update nested object field``() =
        let json = Json.Object [
            "person", Json.Object [
                "name", Json.String "John"
                "age", Json.Number 30.0
            ]
            "active", Json.True
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "person"; FieldKey "age"] -> Some (Json.Number 31.0)
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "person", Json.Object [
                "name", Json.String "John"
                "age", Json.Number 31.0
            ]
            "active", Json.True
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update multiple values``() =
        let json = Json.Array [Json.Number 1.0; Json.Number 2.0; Json.Number 3.0]
        let chooser path value =
            match value with
            | Json.Number n -> Some (Json.Number (n * 2.0))
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Array [Json.Number 2.0; Json.Number 4.0; Json.Number 6.0]
        Should.equal expected result

    [<Fact>]
    member _.``update with condition based on value``() =
        let json = Json.Object [
            "scores", Json.Array [Json.Number 95.0; Json.Number 88.0; Json.Number 76.0]
        ]
        let chooser path value =
            match value with
            | Json.Number n when n > 90.0 -> Some (Json.Number 100.0)
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "scores", Json.Array [Json.Number 100.0; Json.Number 88.0; Json.Number 76.0]
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update complex structure``() =
        let json = Json.Object [
            "users", Json.Array [
                Json.Object ["name", Json.String "Alice"; "score", Json.Number 95.0]
                Json.Object ["name", Json.String "Bob"; "score", Json.Number 88.0]
            ]
            "metadata", Json.Object ["count", Json.Number 2.0]
        ]
        
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "users"; ArrayIndex 1; FieldKey "score"] -> Some (Json.Number 92.0)
            | [FieldKey "metadata"; FieldKey "count"] -> Some (Json.Number 3.0)
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "users", Json.Array [
                Json.Object ["name", Json.String "Alice"; "score", Json.Number 95.0]
                Json.Object ["name", Json.String "Bob"; "score", Json.Number 92.0]
            ]
            "metadata", Json.Object ["count", Json.Number 3.0]
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update boolean values``() =
        let json = Json.Object [
            "isActive", Json.True
            "isAdmin", Json.False
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "isAdmin"] -> Some Json.True
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "isActive", Json.True
            "isAdmin", Json.True
        ]
        Should.equal expected result

    [<Fact>]
    member _.``update null value``() =
        let json = Json.Object [
            "data", Json.Null
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "data"] -> Some (Json.String "not null")
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "data", Json.String "not null"
        ]
        Should.equal expected result

    [<Fact>]
    member _.``no update when chooser returns None``() =
        let json = Json.Object [
            "name", Json.String "John"
            "age", Json.Number 30.0
        ]
        let chooser path value = None
        
        let result = JsonPath.update json chooser
        Should.equal json result

    [<Fact>]
    member _.``update empty array``() =
        let json = Json.Array []
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [] -> Some (Json.Array [Json.String "added"])
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Array [Json.String "added"]
        Should.equal expected result

    [<Fact>]
    member _.``update empty object``() =
        let json = Json.Object []
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [] -> Some (Json.Object ["newField", Json.String "value"])
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object ["newField", Json.String "value"]
        Should.equal expected result

    [<Fact>]
    member _.``test path order is correct``() =
        let json = Json.Object [
            "items", Json.Array [
                Json.Object ["value", Json.Number 1.0]
            ]
        ]
        
        let capturedPaths = System.Collections.Generic.List<obj list>()
        
        let chooser path value =
            capturedPaths.Add(path)
            None
        
        let _ = JsonPath.update json chooser
        
        // 验证路径顺序是从根到叶子
        let expectedPaths = [
            []  // 根节点
            [box "items"]
            [box "items"; box 0]
            [box "items"; box 0; box "value"]
        ]
        
        Should.equal expectedPaths (List.ofSeq capturedPaths)

    [<Fact>]
    member _.``update mixed path types``() =
        let json = Json.Object [
            "data", Json.Array [
                Json.Object ["name", Json.String "test"]
            ]
        ]
        let chooser path value =
            match List.map JsonKeyIndex.from path with
            | [FieldKey "data"; ArrayIndex 0; FieldKey "name"] -> Some (Json.String "updated")
            | _ -> None
        
        let result = JsonPath.update json chooser
        let expected = Json.Object [
            "data", Json.Array [
                Json.Object ["name", Json.String "updated"]
            ]
        ]
        Should.equal expected result