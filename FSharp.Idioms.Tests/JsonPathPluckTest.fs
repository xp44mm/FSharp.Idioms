namespace FSharp.Idioms

open Xunit


open FSharp.Idioms.Jsons

type JsonPathPluckTest(output: ITestOutputHelper) =
    [<Fact>]
    member _.``pluck with empty path returns original json``() =
        let json = Json.String "test"
        let result = JsonPath.pluck json []
        Assert.Equal(Some json, result)

    [<Fact>]
    member _.``pluck with array path and valid index returns element``() =
        let innerJson = Json.String "inner"
        let json = Json.Array [ Json.String "first"; innerJson; Json.String "third" ]
        let result = JsonPath.pluck json [ box 1 ]
        Assert.Equal(Some innerJson, result)

    [<Fact>]
    member _.``pluck with array path and out of bounds index returns None``() =
        let json = Json.Array [ Json.String "first"; Json.String "second" ]
        let result = JsonPath.pluck json [ box 5 ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with array path and negative index returns None``() =
        let json = Json.Array [ Json.String "first"; Json.String "second" ]
        let result = JsonPath.pluck json [ box -1 ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with object path and existing property returns value``() =
        let innerJson = Json.Number 42.0
        let json = Json.Object [ ("name", Json.String "John"); ("age", innerJson) ]
        let result = JsonPath.pluck json [ box "age" ]
        Assert.Equal(Some innerJson, result)

    [<Fact>]
    member _.``pluck with object path and non-existing property returns None``() =
        let json = Json.Object [ ("name", Json.String "John") ]
        let result = JsonPath.pluck json [ box "age" ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with nested path returns deep element``() =
        let deepest = Json.True
        let innerObject = Json.Object [ ("deep", deepest) ]
        let json = Json.Object [ ("level1", Json.Array [ innerObject ]) ]

        let result = JsonPath.pluck json [ box "level1"; box 0; box "deep" ]
        Assert.Equal(Some deepest, result)

    [<Fact>]
    member _.``pluck with invalid path type returns None``() =
        let json = Json.Object [ "name", Json.String "John" ]
        let result = JsonPath.pluck json [ box "name"; box "invalid" ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with non-array/non-object json returns None``() =
        let json = Json.String "simple"
        let result = JsonPath.pluck json [ box "anyKey" ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with complex nested path works correctly``() =
        let targetValue = Json.Number 100.0

        let json =
            Json.Object
                [ ("users",
                   Json.Array
                       [ Json.Object
                             [ ("id", Json.Number 1.0)
                               ("scores", Json.Array [ Json.Object [ ("math", targetValue) ] ]) ] ]) ]

        let path = [ box "users"; box 0; box "scores"; box 0; box "math" ]
        let result = JsonPath.pluck json path
        Assert.Equal(Some targetValue, result)

    [<Fact>]
    member _.``pluck with wrong type in path returns None for array``() =
        let json = Json.Array [ Json.String "test" ]
        let result = JsonPath.pluck json [ box "notAnIndex" ]
        Assert.Equal(None, result)

    [<Fact>]
    member _.``pluck with wrong type in path returns None for object``() =
        let json = Json.Object [ ("key", Json.String "value") ]
        let result = JsonPath.pluck json [ box 123 ]
        Assert.Equal(None, result)
