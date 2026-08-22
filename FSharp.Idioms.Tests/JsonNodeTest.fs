namespace FSharp.Idioms

open FSharp.Idioms.Jsons

open Xunit
open FSharp.xUnit

open System.Text.Json.Nodes

type JsonNodeTest(output: ITestOutputHelper) =
    // 辅助函数：测试往返转换（DU -> JsonNode -> DU）
    let roundTrip (original: Json) =
        let node = JsonNode.fromDU original
        let result = JsonNode.toDU node
        Should.equal original result

    // 辅助：测试从字符串解析的JsonNode转DU再转回去
    let parseAndRoundTrip (jsonText: string) =
        let node = JsonNode.Parse(jsonText)
        let du = JsonNode.toDU node
        let node2 = JsonNode.fromDU du
        let jsonText2 = node2.ToJsonString()
        // 比较标准化后的字符串（忽略格式差异）
        let expected = JsonNode.Parse(jsonText).ToJsonString()
        Should.equal expected jsonText2

    [<Fact>]
    member this.``01 - Null roundtrip``() =
        let du = Json.Null
        roundTrip du

    [<Fact>]
    member this.``02 - Boolean true roundtrip``() =
        let du = Json.True
        roundTrip du

    [<Fact>]
    member this.``03 - Boolean false roundtrip``() =
        let du = Json.False
        roundTrip du

    [<Fact>]
    member this.``04 - String roundtrip``() =
        let du = Json.String "hello"
        roundTrip du

    [<Fact>]
    member this.``05 - Number integer roundtrip``() =
        let du = Json.Number 42.0
        roundTrip du

    [<Fact>]
    member this.``06 - Number floating roundtrip``() =
        let du = Json.Number 3.141592653589793
        roundTrip du

    [<Fact>]
    member this.``07 - Number negative roundtrip``() =
        let du = Json.Number -273.15
        roundTrip du

    [<Fact>]
    member this.``08 - Empty object roundtrip``() =
        let du = Json.Object []
        roundTrip du

    [<Fact>]
    member this.``09 - Simple object roundtrip``() =
        let du =
            Json.Object
                [
                    "name", Json.String "Alice"
                    "age", Json.Number 30.0
                    "active", Json.True
                ]
        roundTrip du

    [<Fact>]
    member this.``10 - Nested object roundtrip``() =
        let du =
            Json.Object
                [
                    "user",
                    Json.Object
                        [
                            "id", Json.Number 1.0
                            "profile",
                            Json.Object
                                [
                                    "email", Json.String "a@b.com"
                                    "verified", Json.False
                                ]
                        ]
                ]
        roundTrip du

    [<Fact>]
    member this.``11 - Empty array roundtrip``() =
        let du = Json.Array []
        roundTrip du

    [<Fact>]
    member this.``12 - Simple array roundtrip``() =
        let du =
            Json.Array
                [
                    Json.Number 1.0
                    Json.String "two"
                    Json.True
                    Json.Null
                ]
        roundTrip du

    [<Fact>]
    member this.``13 - Array of objects roundtrip``() =
        let du =
            Json.Array
                [
                    Json.Object [ "x", Json.Number 1.0 ]
                    Json.Object [ "y", Json.Number 2.0 ]
                ]
        roundTrip du

    [<Fact>]
    member this.``14 - Complex nested structure roundtrip``() =
        let du =
            Json.Object
                [
                    "top",
                    Json.Array
                        [
                            Json.Object
                                [
                                    "a", Json.String "foo"
                                    "b", Json.Array [ Json.Number 1.0; Json.Null ]
                                ]
                            Json.False
                        ]
                    "extra", Json.Object []
                ]
        roundTrip du

    // 以下测试从 JSON 文本解析，再转 DU，再转回节点，比较序列化结果（确保与标准库兼容）

    [<Fact>]
    member this.``15 - Parse simple JSON text``() =
        parseAndRoundTrip """{"name":"Alice","age":30}"""

    [<Fact>]
    member this.``16 - Parse array JSON text``() =
        parseAndRoundTrip """[1, "two", true, null]"""

    [<Fact>]
    member this.``17 - Parse nested JSON text``() =
        parseAndRoundTrip """{"a":{"b":[1,2,3]},"c":false}"""

    [<Fact>]
    member this.``18 - Parse with escaped characters``() =
        parseAndRoundTrip """{"msg":"Hello\nWorld"}"""

    [<Fact>]
    member this.``19 - Parse Unicode``() = parseAndRoundTrip """{"emoji":"😊"}"""

    // 测试边缘情况：空字符串、零、大数字等
    [<Fact>]
    member this.``20 - Edge cases``() =
        let du =
            Json.Object
                [
                    "empty", Json.String ""
                    "zero", Json.Number 0.0
                    "max", Json.Number 1.7976931348623157e+308
                    "min", Json.Number -1.7976931348623157e+308
                ]
        roundTrip du

    // 测试 fromDU 生成 JsonNode 后，能否通过 JsonNode 的标准 API 访问
    [<Fact>]
    member this.``21 - Access JsonNode properties after fromDU``() =
        let du =
            Json.Object
                [
                    "a", Json.String "value"
                    "b", Json.Array [ Json.Number 1.0; Json.Number 2.0 ]
                ]
        let node = JsonNode.fromDU du
        let a = node["a"].GetValue<string>()
        Should.equal "value" a
        let b = node["b"].AsArray()
        Should.equal 2 b.Count
        let first = b[0].GetValue<double>()
        Should.equal 1.0 first
