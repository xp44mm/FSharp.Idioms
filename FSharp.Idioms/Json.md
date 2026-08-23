# FSharp.Idioms.Json 用法说明

本项目的 JSON 功能以 F# 判别联合（DU）`FSharp.Idioms.Jsons.Json` 为核心，辅以若干配套模块。本说明从 `FSharp.Idioms\JsonNode.fs` 入手，覆盖整个 JSON 工具集：

| 模块（源码文件） | 作用 |
| --- | --- |
| `FSharp.Idioms.Jsons.Json`（`Jsons\Json.fs`） | JSON 数据模型：六种情况的判别联合 |
| `FSharp.Idioms.Json`（`Json.fs`） | 常用函数封装：构造、打印、对象互转 |
| `FSharp.Idioms.Jsons.JsonRender`（`Jsons\JsonRender.fs`） | 输出 JSON 字符串 |
| `FSharp.Idioms.JsonNode`（`JsonNode.fs`） | 与 `System.Text.Json.JsonNode` 互转 |
| `FSharp.Idioms.Jsons.JsonReaders` / `JsonWriters`（`Jsons\*.fs`） | 与 .NET/F# 对象互转（反射） |
| `FSharp.Idioms.JsonPath`（`JsonPath.fs`） | 按路径查询 / 枚举 / 更新 |
| `FSharp.Idioms.JsonString`（`JsonString.fs`） | 字符串加引号 / 去引号 |
| `FSharp.Idioms.JsonNumber`（`JsonNumber.fs`） | 解析 JSON 数字 |

---

## 一、Json 数据模型

```fsharp
namespace FSharp.Idioms.Jsons

[<RequireQualifiedAccess>]
type Json =
    | Object of entries: list<string * Json>
    | Array of elements: list<Json>
    | Null
    | False
    | True
    | String of text: string
    | Number of value: float // 15~17 位有效数字 max 10^308
```

要点：

- 数字统一用 `float` 保存（`int` 等整型构造时转成 `float`）；
- `Object` 的条目是 `string * Json` 列表，**按键顺序保留**，允许重名（合并时后见者胜）；
- 使用前 `open FSharp.Idioms.Jsons`；模块 `FSharp.Idioms.Json` 中的函数用 `open FSharp.Idioms`。

## 二、构造 Json

直接构造：

```fsharp
open FSharp.Idioms.Jsons

Json.Null
Json.True
Json.False
Json.String "hello"
Json.Number 3.14
Json.Object [ "name", Json.String "Alice"; "age", Json.Number 30.0 ]
Json.Array [ Json.Number 1.0; Json.String "two"; Json.True; Json.Null ]
```

`Json.just` 便捷构造（类型由参数推断）：

```fsharp
Json.just ()        // Json.Null
Json.just true      // Json.True
Json.just "abc"     // Json.String "abc"
Json.just 42        // Json.Number 42.0（int 自动转 float）
Json.just 3.14      // Json.Number 3.14
Json.just [ "a", Json.Number 1.0 ]        // Json.Object [("a", Json.Number 1.0)]
Json.just [ Json.Number 1.0; Json.True ]  // Json.Array [Json.Number 1.0; Json.True]
```

`Json.boolean`（模块 `FSharp.Idioms.Json`）：

```fsharp
open FSharp.Idioms

Json.boolean true   // Json.True
Json.boolean false  // Json.False
```

## 三、读取与访问

模式匹配：

```fsharp
let describe json =
    match json with
    | Json.Null -> "null"
    | Json.True | Json.False -> "bool"
    | Json.String _ -> "string"
    | Json.Number _ -> "number"
    | Json.Array _ -> "array"
    | Json.Object _ -> "object"
```

索引器（对象按键，数组按下标）：

```fsharp
let json =
    Json.Object [
        "name", Json.String "Alice"
        "tags", Json.Array [ Json.String "a"; Json.String "b" ]
    ]

json.["name"]       // Json.String "Alice"
json.["tags"].[0]   // Json.String "a"
// 注意：键不存在会抛异常，先用 hasProperty 判断，或用 JsonPath.pluck 安全取值
```

常用成员：

```fsharp
json.hasProperty "name"      // bool
json.entries                 // list<string * Json>（仅 Object）
json.elements                // list<Json>（仅 Array）
json.["name"].stringText     // string（仅 String）
json.["age"].floatValue      // float（仅 Number）
json.["active"].boolValue    // bool（仅 True / False）
```

## 四、修改与合并（不可变）

```fsharp
// assign：合并对象，后见者胜，保持先来者顺序
let a = Json.Object [ "x", Json.Number 1.0; "y", Json.Number 2.0 ]
let b = Json.Object [ "y", Json.Number 9.0; "z", Json.Number 3.0 ]
a.assign(b)   // Json.Object [ "x",1.0; "y",9.0; "z",3.0 ]

// coalesce：先见者胜，后来者补缺
a.coalesce(b) // Json.Object [ "x",1.0; "y",2.0; "z",3.0 ]

// setProperty：替换已有键的值（键不存在时无变化）
a.setProperty("y", Json.Number 8.0)

// setElement：替换数组元素（下标越界时无变化）
(Json.Array [ Json.Number 1.0; Json.Number 2.0 ]).setElement(0, Json.Number 9.0)
```

按路径递归更新用 `JsonPath.update`，见第八节。

## 五、输出字符串

```fsharp
open FSharp.Idioms

let json = Json.Object [ "name", Json.String "Alice"; "age", Json.Number 30.0 ]

Json.print json
// {"name":"Alice","age":30}
```

- `Json.print : Json -> string`：标准 JSON。数字用 `"G15"` 不变文化格式；NaN / Infinity 输出 `null`；
- `Json.printUnquotedJson : Json -> string`：省略无歧义引号的“宽松 JSON”；
- `Json.quote : string -> string` / `Json.unquote : string -> string`：字符串加 / 去 JSON 引号与转义（`\n`、`\t`、`\uXXXX` 等）；
- `JsonNumber.tryParse : string -> float option`：按 JSON 语法解析数字字符串（支持小数与指数）。

## 六、与 System.Text.Json.JsonNode 互转（JsonNode.fs）

`FSharp.Idioms.JsonNode` 模块提供两个互逆函数：

- `fromDU : Json -> JsonNode`（本项目的 DU → 标准库 JsonNode）
- `toDU : JsonNode -> Json`（标准库 JsonNode → 本项目的 DU）

典型用法一：解析 JSON 文本为 DU。

```fsharp
open System.Text.Json.Nodes

let text = """{"name":"Alice","age":30,"tags":["a","b"]}"""
let node = JsonNode.Parse(text)      // System.Text.Json.Nodes.JsonNode
let json = JsonNode.toDU node        // FSharp.Idioms.Jsons.Json
```

典型用法二：DU 输出为 JSON 文本。

```fsharp
let json = Json.Object [ "name", Json.String "Alice" ]
let node = JsonNode.fromDU json
let text = node.ToJsonString()       // {"name":"Alice"}
```

也可以转换后用 JsonNode 的标准 API 访问：

```fsharp
let node = JsonNode.fromDU json
let name = node["name"].GetValue<string>()
let arr  = node["tags"].AsArray()
arr.Count                            // 2
```

注意：

- 数字经 `toDU` 一律读成 `float`（`GetValue<double>`），超出 double 精度的大整数会丢失精度；
- `toDU` 遇到不支持的 `JsonValueKind` 会抛异常；
- `fromDU` 中 `Json.Null` 必须用 `JsonValue.Create<obj>(null)` 才能表示为 `null`，源码中已处理。

## 七、与 .NET / F# 对象互转（Json.from / Json.deserialize）

模块 `FSharp.Idioms.Json`：

```fsharp
open FSharp.Idioms

let from<'t> (value: 't) : Json            // 't -> Json
let deserialize<'t> (json: Json) : 't      // Json -> 't
let fromObj : Type -> obj -> Json          // 动态版本
let toObj : Type -> Json -> obj            // 动态版本
```

类型映射（摘自 `Jsons\JsonReaders.fs` / `Jsons\JsonWriters.fs`）：

| F# / .NET 类型 | Json 表示 |
| --- | --- |
| `bool` | `True` / `False` |
| `char` | `String`（单字符） |
| `sbyte` `byte` `int16` `uint16` `int` `uint32` `single` `float` | `Number` |
| `int64` `uint64` `decimal` `nativeint` `unativeint` | `String`（保证精度） |
| `DateTime` | `String`（`"o"` 往返格式） |
| `unit` | `Array []` |
|`None` | `Array []`|
|`Some x` | `Array [x]` |
| `Nullable` | 值本身 |
| `Null` | 值本身 |
| 枚举 |`String "成员名"`|
| `[<Flags>]`枚举|`Array [成员名...]` |
| 数组 、 `IEnumerable` | `Array` |
| 元组 | `Array` |
| `Map` | `Array [ [k; v]; ... ]` |
| 记录 | `Object`（字段名做键） |
| 可区分联合 | （见分项）| 
| - 无字段：`String "CaseName"`|
| - 有字段：`Array ["CaseName"; 字段...]`|
| - `[<RequireQualifiedAccess>]` | 带类型名前缀 `"类型名.CaseName"` |
| 类 | `Object`（可读写的公开属性） |

示例：

```fsharp
type Person = { Name: string; Age: int }

type Shape =
    | Circle of radius: float
    | Rect of width: float * height: float
    | Empty

Json.from { Name = "Alice"; Age = 30 }
// Json.Object [ "Name", Json.String "Alice"; "Age", Json.Number 30.0 ]

Json.from (Circle 2.5)
// Json.Array [ Json.String "Circle"; Json.Number 2.5 ]

Json.from Empty
// Json.String "Empty"

Json.from (Some 42)       // Json.Array [ Json.Number 42.0 ]
Json.from None            // Json.Array []
Json.from 42L             // Json.String "42"
Json.from (Map.ofList ["a", 1; "b", 2])
// Json.Array [ Json.Array [Json.String "a"; Json.Number 1.0]; ... ]

// 反向还原
Json.deserialize<Person> (
    Json.Object [ "Name", Json.String "Alice"; "Age", Json.Number 30.0 ])
// { Name = "Alice"; Age = 30 }
```

注意：记录反序列化时缺失的字段填零值（`Zeros` 工具）；联合用例标签必须与源码中的用例名（含限定前缀）一致。

## 八、JsonPath 查询与更新

```fsharp
open FSharp.Idioms.Jsons

let json =
    Json.Object [
        "user", Json.Object [
            "name", Json.String "Bob"
            "scores", Json.Array [ Json.Number 95.0; Json.Number 88.0 ]
        ]
    ]

// pluck：按路径取值，路径元素为 string（对象键）或 int（数组下标）
JsonPath.pluck json [ "user"; "name" ]       // Some (Json.String "Bob")
JsonPath.pluck json [ "user"; "scores"; 1 ]  // Some (Json.Number 88.0)
JsonPath.pluck json [ "user"; "age" ]        // None

// enumerate：枚举所有叶子节点，返回 (路径, 值) 序列
for path, value in JsonPath.enumerate json do
    printfn "%A -> %s" path (Json.print value)

// update：按路径递归更新，chooser 返回 Some 时替换该节点
let updated =
    JsonPath.update json (fun path value ->
        match value with
        | Json.Number n when n > 90.0 -> Some (Json.Number 100.0)
        | _ -> None
    )
```

`JsonPath.update` 的 chooser 收到的 `path` 是 `obj list`，可先用 `List.map JsonKeyIndex.from path` 转成 `JsonKeyIndex`（`ArrayIndex` / `FieldKey`）再匹配，例如：

```fsharp
JsonPath.update json (fun path value ->
    match List.map JsonKeyIndex.from path with
    | [FieldKey "user"; ArrayIndex 0; FieldKey "name"] -> Some (Json.String "Alice")
    | _ -> None)
```

## 九、完整示例：解析 → 读取 → 修改 → 输出

```fsharp
open FSharp.Idioms
open FSharp.Idioms.Jsons
open System.Text.Json.Nodes

// 1. 解析文本为 DU
let text = """{"user":{"name":"Bob","scores":[95,88,76]}}"""
let json = text |> JsonNode.Parse |> JsonNode.toDU

// 2. 读取
let name  = json.["user"].["name"].stringText         // "Bob"
let first = json.["user"].["scores"].[0].floatValue   // 95.0

// 3. 更新：90 分以上的成绩改成 100
let updated =
    JsonPath.update json (fun path value ->
        match value with
        | Json.Number n when n > 90.0 -> Some (Json.Number 100.0)
        | _ -> None)

// 4. 输出
let text2 =
    updated
    |> JsonNode.fromDU
    |> fun node -> node.ToJsonString()
// {"user":{"name":"Bob","scores":[100,88,76]}}
```

---

## 附：相关测试参考

- `FSharp.Idioms.Tests\JsonNodeTest.fs`：DU ↔ JsonNode 往返与文本解析
- `FSharp.Idioms.Tests\JsonTest.fs`：`Json.boolean`、`Json.tryCapture`
- `FSharp.Idioms.Tests\JsonPathPluckTest.fs` / `JsonPathEnumerateTest.fs` / `JsonPathUpdateTest.fs`：路径查询、枚举、更新
- `FSharp.Idioms.Tests\JsonStringTest.fs` / `JsonNumberTest.fs`：字符串转义与数字解析
