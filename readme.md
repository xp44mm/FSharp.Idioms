`FSharp.Idioms` is a library of functions for FSharp.

---

# Json DU 用法说明

`FSharp.Idioms.Jsons.Json` 是本库的 JSON 内存模型：**不可变、保序、基于 list** 的判别联合（DU）。库内所有 JSON 能力（反射互转、渲染、路径操作）都围绕它展开。

- 源码：`FSharp.Idioms/Jsons/Json.fs`
- 命名空间：`FSharp.Idioms.Jsons`
- 渲染：`JsonRender`；反射互转：`JsonReaderApp` / `JsonWriterApp`（门面在 `FSharp.Idioms.Json` 模块）

## 1. 结构

```fsharp
[<RequireQualifiedAccess>]
type Json =
    | Object of entries : list<string * Json>   // 保序键值对
    | Array  of elements: list<Json>
    | Null
    | False
    | True
    | String of text : string
    | Number of value: float                    // 15~17 位有效数字，上限 10^308
```

设计要点：

- `RequireQualifiedAccess`：必须写 `Json.Object` / `Json.Null`，不能裸写 `Null`，避免与业务 DU 的 case 名冲突。
- `True` / `False` 拆成两个无字段 case，`match` 时不必再嵌套判断 `bool`。
- 对象用 `(string * Json) list` 而不是 `Map`：**保序**、允许重复键、键区分大小写。
- `Number` 只有 `float` 一种载体，精度陷阱见第 8 节。

## 2. 构造

两种方式，等价：

```fsharp
// 直接构造
let a = Json.Object [ "name", Json.String "guxin"; "age", Json.Number 18.0 ]

// 用 just 重载
let b = Json.just [ "name", Json.just "guxin"; "age", Json.just 18 ]
```

`Json.just` 重载表：

| 参数类型 | 结果 |
| --- | --- |
| `()` 无参 | `Json.Null` |
| `bool` | `Json.True` / `Json.False` |
| `string` | `Json.String` |
| `float` | `Json.Number` |
| `int` | `Json.Number (float value)` |
| `list<string * Json>` | `Json.Object` |
| `[<ParamArray>] (string * Json)[]` | `Json.Object` |
| `list<Json>` | `Json.Array` |
| `[<ParamArray>] Json[]` | `Json.Array` |

嵌套示例：

```fsharp
let user =
    Json.just [
        "name", Json.just "guxin"
        "age",  Json.just 18
        "vip",  Json.just true
        "tags", Json.just [ Json.just "a"; Json.just "b" ]
        "note", Json.just ()
    ]
// {"name":"guxin","age":18,"vip":true,"tags":["a","b"],"note":null}
```

注意：`Json.just []` 无法推断重载（`list<string * Json>` 与 `list<Json>` 有二义性），需写成 `Json.just ([]: list<Json>)` 或直接用 `Json.Array []`。

## 3. 读取

### 3.1 索引器与访问器

| 成员 | 适用 case | 类型不符时 |
| --- | --- | --- |
| `t.[i]`（int） | `Array` | `failwith "int index is only for array."` |
| `t.[key]`（string） | `Object` | 键不存在 → `failwith "no found key."`；非 Object → `failwith "string index is only for object."` |
| `getValue()` / `floatValue` | `Number` | `failwith "only for Json.Number"` |
| `getText()` / `stringText` | `String` | `failwith "only for Json.String"` |
| `getBooleanValue()` / `boolValue` | `True` / `False` | `failwith "only for Json.Boolean"` |
| `getElements()` / `elements` | `Array` | `ArgumentOutOfRangeException "only for Json.Array"` |
| `getEntries()` / `entries` | `Object` | `ArgumentOutOfRangeException "only for Json.Object"` |
| `hasProperty key` | `Object` | **不抛异常**，非 Object 返回 `false` |

```fsharp
let n = Json.Number 2.0
n.getValue()              // 2.0
Json.Object ["age", n].["age"].getValue()   // 2.0
```

### 3.2 推荐：模式匹配

访问器在 case 不符时抛异常，正式代码优先用模式匹配，把失败显式化：

```fsharp
let tryField (key: string) (json: Json) =
    match json with
    | Json.Object entries ->
        entries |> List.tryFind (fst >> (=) key) |> Option.map snd
    | _ -> None

let tryInt (json: Json) =
    match json with
    | Json.Number x -> Some x
    | _ -> None
```

## 4. 不可变更新

所有更新操作**返回新值**，旧值不变（list 结构共享，拷贝代价低）。

| 成员 | 语义 |
| --- | --- |
| `setProperty(k, v)` | 只替换**已存在**的键；键不存在时**原样返回，不会新增** |
| `object.assign(entries)` | 合并，**后来者赢**（同键取后者值），键顺序按先到者 |
| `obj1.assign(obj2)` | 同上，以 `obj2.entries` 为来源 |
| `object.coalesce(entries)` | 合并，**先来者赢**，后来者只补充缺失键 |
| `obj1.coalesce(obj2)` | 同上 |
| `array.setElement(i, v)` | 替换数组第 i 个元素；越界不报错，静默返回原数组 |

```fsharp
let target = Json.just [ "name", Json.just "guxin"; "age", Json.just 18 ]
let source = Json.just [ "state", Json.just "single"; "age", Json.just 22 ]

target.assign(source).entries
// ["name", "guxin"; "age", 22; "state", "single"]

target.coalesce(source).entries
// ["name", "guxin"; "age", 18; "state", "single"]
```

已废弃（`[<Obsolete>]`，用 `assign` 代替）：

- `addProperty(k, v)`：前置插入，不覆盖同名键。
- `replaceProperty(k, v)`：存在则替换、不存在则追加（`setProperty` + 追加的旧写法）。

## 5. 渲染

| 函数 | 说明 |
| --- | --- |
| `JsonRender.stringifyNormalJson` | 标准 JSON，紧凑（无空格、无换行） |
| `FSharp.Idioms.Json.print` | 同上，门面别名 |
| `JsonRender.stringifyUnquotedJson` | 非严格 JSON：**键**在无歧义时省略引号 |
| `FSharp.Idioms.Json.printUnquotedJson` | 同上，门面别名 |

```fsharp
JsonRender.stringifyNormalJson   user   // {"name":"guxin","age":18,"vip":true,...}
JsonRender.stringifyUnquotedJson user   // {name:"guxin",age:18,vip:true,...}
```

细节：

- **键去引号**条件见 `stringifyKey`：为空、含 `, : { } [ ] "` 或控制字符、首尾空格时必须加引号。
- `unquoted` 模式下**字符串值仍然一律加引号**（不会与数字/布尔混淆）。若需要值也去引号，可自行用 `stringifyStringText` 判断后再拼接。
- `Number` 渲染：`normal` 用 `"G15"` + `InvariantCulture`，`NaN` / `±Infinity` 输出 `null`；`unquoted` 用 `Convert.ToString`（受当前区域性影响，`NaN` 会输出非法的 `NaN`）。
- 字符串转义在 `JsonString.quote` / `JsonString.unquote`（`FSharp.Idioms.Json.quote` / `unquote`）。

## 6. 与 F# / .NET 类型互转

```fsharp
Json.from<'t>        : 't -> Json          // 反射读取，JsonReaderApp.readDynamic
Json.fromObj         : Type -> obj -> Json
Json.deserialize<'t> : Json -> 't          // 反射写入，JsonWriterApp.writeDynamic
Json.toObj           : Type -> Json -> obj
```

```fsharp
type Person = { name: string; age: int }

let json = Json.from ({ name = "guxin"; age = 18 }: Person)   // Json.Object [...]
let p    = Json.deserialize<Person> json                      // { name = "guxin"; age = 18 }
```

编码约定：

| 类型 | 编码 |
| --- | --- |
| `bool` | `True` / `False` |
| `char` | `String`（单字符） |
| `sbyte` `byte` `int16` `uint16` `int` `uint32` `float` | `Number` |
| `single` | `Number`（先 `Math.Round(_, 8)`） |
| `int64` `uint64` `decimal` `nativeint` `unativeint` | **`String`**（避免 float 精度丢失） |
| `DateTime` | `String`，`"o"` 往返格式 |
| `unit` | `Array []`（空数组） |
| `DBNull` | `Null` |
| `Option<'T>` | `None` → `[]`；`Some v` → `[v]`（**空数组即 None**） |
| `Nullable<'T>` | `null` → `Null`；否则等于 `'T` 的编码 |
| 枚举 | `String` 名称；`[<Flags>]` → 名称数组 |
| 元组 | `Array`，按位置定长 |
| 数组 / `list` / `Set` / `HashSet` | `Array` |
| `Map<K,V>` / `IEnumerable<KeyValuePair<K,V>>` | `Array of [k, v]` |
| record | `Object`（字段名 = 属性名） |
| union | 无字段 → `String "Case"`；有字段 → `Array ["Case"; f1; f2; ...]` |
| class | `Object`（公共可读写属性） |
| `Json` 自身 | 原样返回（`tryJson` 优先于 `tryUnion`） |

补充说明：

- union 的 tag 由 `UnionType.getQualifiedAccess` 生成（限定访问前缀 + case 名），反序列化要求 tag 完全一致。
- 反序列化 record / class 时，**缺失属性取 `ZeroUtils.getZero` 默认值**，不抛异常；但集合、元组元素个数不匹配仍会抛异常。
- 分派顺序写在 `JsonReaderApp.readers` / `JsonWriterApp.writers`，`tryClass` 兜底放最后，所以 list / set / map / 枚举分支优先于 class 分支。需要新增类型支持时，在这两个列表里插位置即可。

## 7. 与 System.Text.Json 互转

```fsharp
JsonElement.toDU  : JsonElement  -> Json   // FSharp.Idioms.JsonElement
JsonDocument.toDU : JsonDocument -> Json   // FSharp.Idioms.JsonDocument
JsonNode.fromDU   : Json -> JsonNode       // FSharp.Idioms.JsonNode
JsonNode.toDU     : JsonNode -> Json
```

```fsharp
use doc = System.Text.Json.JsonDocument.Parse """{"a":[1,2]}"""
let json = JsonDocument.toDU doc
JsonRender.stringifyNormalJson json   // {"a":[1,2]}
```

## 8. 注意事项

1. **数值精度**：`Number` 是 `float`，超过 2^53 的整数、金额等高精度小数请走 `Json.String`（或借第 6 节 int64 / decimal 的自动编码）。
2. **构造不查重**：`Json.Object` 不去重也不排序，重复键会原样保留；只有 `assign` / `coalesce` 才按键去重。渲染时重复键会全部输出，取值时 `t.[key]` 取第一个匹配。
3. **`setProperty` 不新增键**：想"有则改、无则加"请用 `assign([k, v])`。
4. **访问器会抛异常**：除 `hasProperty` 外，case 不符或键不存在都会抛；不信任的输入先用 `hasProperty` / `match` 判断。
5. **键区分大小写**：`hasProperty "Name"` 对 `{"name": ...}` 返回 `false`。
6. **List 语义**：`Array` 的 `setElement` 越界静默失败，`t.[i]` 越界由 `List` 自身抛 `ArgumentOutOfRangeException`。
