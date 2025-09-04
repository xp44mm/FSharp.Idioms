namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions

open FSharp.Idioms.Jsons
open FSharp.xUnit

type JsonPathEnumerateTest(output: ITestOutputHelper) =
    
    [<Fact>]
    member _.``enumerate string value``() =
        let json = Json.String "test"
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [([], Json.String "test")]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate number value``() =
        let json = Json.Number 42.0
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [([], Json.Number 42.0)]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate true value``() =
        let json = Json.True
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [([], Json.True)]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate false value``() =
        let json = Json.False
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [([], Json.False)]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate null value``() =
        let json = Json.Null
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [([], Json.Null)]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate simple array``() =
        let json = Json.Array [Json.String "a"; Json.String "b"]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box 0], Json.String "a")
            ([box 1], Json.String "b")
        ]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate nested array``() =
        let json = Json.Array [
            Json.Array [Json.String "a"; Json.String "b"]
            Json.String "c"
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box 0; box 0], Json.String "a")
            ([box 0; box 1], Json.String "b")
            ([box 1], Json.String "c")
        ]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate simple object``() =
        let json = Json.Object [
            "name", Json.String "John"
            "age", Json.Number 30.0
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box "name"], Json.String "John")
            ([box "age"], Json.Number 30.0)
        ]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate nested object``() =
        let json = Json.Object [
            "person", Json.Object [
                "name", Json.String "John"
                "age", Json.Number 30.0
            ]
            "active", Json.True
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box "person"; box "name"], Json.String "John")
            ([box "person"; box "age"], Json.Number 30.0)
            ([box "active"], Json.True)
        ]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate complex structure``() =
        let json = Json.Object [
            "users", Json.Array [
                Json.Object ["name", Json.String "Alice"; "scores", Json.Array [Json.Number 95.0; Json.Number 88.0]]
                Json.Object ["name", Json.String "Bob"; "scores", Json.Array [Json.Number 76.0; Json.Number 92.0]]
            ]
            "metadata", Json.Object ["count", Json.Number 2.0]
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box "users"; box 0; box "name"], Json.String "Alice")
            ([box "users"; box 0; box "scores"; box 0], Json.Number 95.0)
            ([box "users"; box 0; box "scores"; box 1], Json.Number 88.0)
            ([box "users"; box 1; box "name"], Json.String "Bob")
            ([box "users"; box 1; box "scores"; box 0], Json.Number 76.0)
            ([box "users"; box 1; box "scores"; box 1], Json.Number 92.0)
            ([box "metadata"; box "count"], Json.Number 2.0)
        ]
        Should.equal exp results

    [<Fact>]
    member _.``enumerate empty array``() =
        let json = Json.Array []
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = []
        Should.equal exp results

    [<Fact>]
    member _.``enumerate empty object``() =
        let json = Json.Object []
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = []
        Should.equal exp results

    [<Fact>]
    member _.``test sequence behavior``() =
        let json = Json.Array [Json.String "a"; Json.String "b"; Json.String "c"]
        let results = JsonPath.enumerate json
        
        // Test that it's truly lazy by taking only first 2 elements
        let firstTwo = results |> Seq.take 2 |> Seq.toList
        let exp = [
            ([box 0], Json.String "a")
            ([box 1], Json.String "b")
        ]
        Should.equal exp firstTwo

    [<Fact>]
    member _.``test path elements are correct types``() =
        let json = Json.Object [
            "items", Json.Array [Json.String "test"]
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        
        let path, value = results.Head
        Should.equal 2 path.Length
        Should.equal (Json.String "test") value
        
        // 验证路径元素类型
        match path.[0] with
        | :? string as s -> Should.equal "items" s
        | _ -> failwith "Expected string"
        
        match path.[1] with
        | :? int as i -> Should.equal 0 i
        | _ -> failwith "Expected int"

    [<Fact>]
    member _.``enumerate with boolean values``() =
        let json = Json.Object [
            "isActive", Json.True
            "isAdmin", Json.False
        ]
        let results = JsonPath.enumerate json |> Seq.toList
        let exp = [
            ([box "isActive"], Json.True)
            ([box "isAdmin"], Json.False)
        ]
        Should.equal exp results