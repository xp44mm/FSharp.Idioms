# FSharp.Idioms.Literal 用法说明

`FSharp.Idioms.Literal` 模块（源码 `Literal.fs`）是一组常用工具的集合，主要提供三方面功能：

1. **打印类型**：把 .NET/F# 的 `Type` 按 **F# 源码写法**输出为字符串；
2. **打印值**：把 .NET/F# 的值按 **F# 源码写法**输出为字符串（多数输出可直接粘贴回 F# 代码重新编译）；
3. **默认值**：根据 `Type` 信息递归地构造出任意类型的“零值”/默认值；
4. **格式化**：按 .NET 格式字符串（如 `"D5"`、`"yyyy-MM-dd"`）格式化值，非 `IFormattable` 的值自动回退到“打印”。

模块本身只是对底层 `FSharp.Idioms.Literals`（打印器）与 `FSharp.Idioms.Zeros`（零值工具）的薄封装。日常使用只需打开模块：

```fsharp
open FSharp.Idioms.Literal
```

## 函数一览

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `stringifyTypeDynamic` | `Type -> string` | 按类型信息打印类型（动态，适合反射场景） |
| `stringifyType<'t>` | `string` | 打印类型的泛型速记 |
| `stringifyDynamic` | `Type -> obj -> string` | 按给定的 `Type` 打印值（动态） |
| `stringify<'t>` | `'t -> string` | 打印值的泛型速记 |
| `defaultofDynamic` | `Type -> obj` | 获取任意类型的默认值（返回 `obj`） |
| `defaultof<'t>` | `'t` | 获取默认值的泛型速记 |
| `formatValue<'T>` | `string -> 'T -> string` | 按格式字符串格式化值 |

---

## 一、打印值：`stringify` / `stringifyDynamic`

### 泛型版本 `stringify`

最基本的用法是 `Literal.stringify<'t> (value:'t)`，返回值是 F# 源码风格的字符串：

```fsharp
Literal.stringify 42                    // "42"
Literal.stringify 3.14                  // "3.14"
Literal.stringify '\t'                  // "'\t'"
Literal.stringify ""                    // "\"\""
Literal.stringify [1;2;3]               // "[1;2;3]"
Literal.stringify [|1;2;3|]             // "[|1;2;3|]"
Literal.stringify ([1;2;3], "x")        // "[1;2;3],\"x\""
Literal.stringify (Some 123)            // "Some 123"
Literal.stringify None                  // "None"
Literal.stringify Set.empty             // "set []"
Literal.stringify Map.empty             // "Map []"
Literal.stringify (System.Nullable())   // "Nullable()"
```

常见类型的输出一览（摘自测试用例）：

| 值 | 输出 |
| --- | --- |
| `0y` / `0uy` / `0s` / `0us` | `"0y"` / `"0uy"` / `"0s"` / `"0us"` |
| `0` / `0u` / `0L` / `0UL` | `"0"` / `"0u"` / `"0L"` / `"0UL"` |
| `0n` / `0un` | `"0n"` / `"0un"` |
| `1.2f` / `1.0f` | `"1.2f"` / `"1f"` |
| `1.2` / `1.0` | `"1.2"` / `"1.0"` |
| `0M` / `0I` | `"0M"` / `"0I"` |
| `[1;2;3]`（列表） | `"[1;2;3]"` |
| `[|1;2;3|]`（数组） | `"[|1;2;3|]"` |
| `Set.ofList [1;2;3]` | `"set [1;2;3]"` |
| `Map ["1",1;"2",2]` | `"Map [\"1\",1;\"2\",2]"` |
| `HashSet [1;2;3]` | `"HashSet [1;2;3]"` |
| `Nullable 3` / `Nullable()` | `"Nullable 3"` / `"Nullable()"` |
| `DBNull.Value` | `"DBNull.Value"` |
| `FileMode.Open` | `"FileMode.Open"` |
| `BindingFlags.Public \|\|\| BindingFlags.NonPublic` | `"BindingFlags.Public\|\|\|BindingFlags.NonPublic"` |
| `typeof<RegexOptions>`（类型本身作为值） | `"typeof<RegexOptions>"` |
| `Guid "936da01f-9abd-4d9d-80c7-02af85c822a8"` | `"Guid(\"936da01f-9abd-4d9d-80c7-02af85c822a8\")"` |
| `DateTimeOffset(2019,9,19,15,18,16,757,TimeSpan(0,8,0,0,0))` | 与左边相同的 F# 源码写法 |
| 匿名记录 `{| name = "xyz"; ``your age`` = 18; order = -1 |}` | `"{name=\"xyz\";order= -1;``your age``=18}"` |

要点：

- 输出是 **F# 源码风格**，绝大多数情况下可以直接粘贴回 F# 文件重新编译；
- 数值类型会带上 F# 字面量后缀（`y`、`uy`、`s`、`us`、`u`、`L`、`UL`、`n`、`un`、`f`、`M`、`I` 等）；
- 枚举打印为 `类型.成员`；带 `[<Flags>]` 的组合枚举打印为 `A|||B`；`None`（即零值）打印为 `类型.None`；
- `null`、`None`、空 `Nullable()` 等“空”值都能正确处理；
- 记录、可区分联合、元组、匿名记录等复合类型会按 F# 语法递归展开。

### 动态版本 `stringifyDynamic`

当静态类型与运行时类型不一致时（典型场景：值被装箱成 `obj`，或者通过反射拿到值），用 `stringifyDynamic` 显式给出 `Type`：

```fsharp
let x: obj = box 42
Literal.stringifyDynamic typeof<int> x   // "42"

let y: obj = null
Literal.stringifyDynamic typeof<obj> y   // "null"
```

规则：

- 若给出的 `Type` 就是 `obj`，会自动改用值的**运行时类型**继续打印；
- 若值为 `null`，输出 `"null"`；
- 若给出的是枚举的**底层类型**（如 `typeof<RegexOptions>.GetEnumUnderlyingType()`），则按底层数值打印（如 `"0"`）。

---

## 二、打印类型：`stringifyType` / `stringifyTypeDynamic`

把 `Type` 打印成 F# 源码风格：

```fsharp
Literal.stringifyType<bool>                          // "bool"
Literal.stringifyType<int[]>                         // "int[]"
Literal.stringifyType<int*string>                    // "int*string"
Literal.stringifyType<(string*int)*(float*bool)>     // "(string*int)*(float*bool)"
Literal.stringifyType<option<int>>                   // "option<int>"
Literal.stringifyType<list<int>>                     // "list<int>"
Literal.stringifyType<Set<int>>                      // "Set<int>"
Literal.stringifyType<Map<int,int>>                  // "Map<int,int>"
Literal.stringifyType<seq<int>>                      // "seq<int>"
Literal.stringifyType<ResizeArray<int>>              // "ResizeArray<int>"
Literal.stringifyType<Type -> string>                // "Type->string"
Literal.stringifyType<{|x:string|}>                  // "{|x:string|}"
Literal.stringifyType<Nullable<int>>                 // "Nullable<int>"
Literal.stringifyType<System.Nullable<_>>.GetGenericTypeDefinition()  // "Nullable<'T>"
```

要点：

- 为照顾 C# 程序员习惯，泛型类型统一采用 `list<int>`、`seq<int>` 这种 `容器<元素>` 写法，而不是 F# 惯用的 `int list`；
- 元组、函数类型、数组、匿名记录、泛型定义（打印为 `Nullable<'T>`）都能处理；
- 动态版本 `stringifyTypeDynamic (ty:Type)` 适合反射等拿不到编译期类型的场景，内部带记忆化（`ConcurrentDictionary`），重复打印同一类型开销很小；
- 若需要自定义打印风格，可以参照 `TypePrinterApp.typePrinters` 列表自行实现并替换打印器。

---

## 三、默认值：`defaultof` / `defaultofDynamic`

根据 `Type` 递归构造“零值”，用于需要“空对象”的场景（例如测试数据、反序列化兜底）：

```fsharp
Literal.defaultof<int>               // 0
Literal.defaultof<char>              // '\u0000'
Literal.defaultof<string>            // null
Literal.defaultof<bool>              // false
Literal.defaultof<int option>        // None
Literal.defaultof<int list>          // []
Literal.defaultof<int Set>           // Set.empty
Literal.defaultof<Map<int,int>>      // Map.empty
Literal.defaultof<int[]>             // [||]
Literal.defaultof<int*float*string>  // (0, 0.0, null)
Literal.defaultof<DBNull>            // DBNull.Value
Literal.defaultof<Nullable<int>>     // Nullable()
Literal.defaultof<BindingFlags>      // 枚举零值
Literal.defaultof<TimeSpan>          // TimeSpan.Zero
```

自定义复合类型同样适用（`ZeroUtils` 会递归处理记录、匿名记录、可区分联合等）：

```fsharp
type Person = { name: string; age: int }

Literal.defaultof<Person>                  // { name = ""; age = 0 }
Literal.defaultof<{| name: string; age: int |}>  // {| name = ""; age = 0 |}
```

要点：

- 泛型版本 `defaultof<'t>` 直接返回强类型值；
- 动态版本 `defaultofDynamic : Type -> obj` 返回 `obj`，需要自行 `:?> 't` 转换，适合反射场景：
  ```fsharp
  let y = Literal.defaultofDynamic typeof<char> :?> char   // '\u0000'
  ```
- 遇到未实现的类型会抛出异常，异常消息中会带有该类型的 F# 源码风格描述；
- 需要补充自定义类型时，可参照 `ZeroUtils.tries` 列表自行实现“零值规则”并扩展框架（框架是递归的）。

---

## 四、格式化值：`formatValue`

`formatValue<'T> (format: string) (value: 'T)` 按 .NET 标准格式字符串格式化值，处理规则按优先级如下：

1. **`string`**：原样返回，不做任何转义或加引号，`format` 参数被忽略；
2. **`IFormattable`**（数值、`DateTime`、`DateTimeOffset`、`TimeSpan`、`Guid` 等）：
   - `format` 为 `null` 或空字符串 → 调用 `value.ToString()`（当前区域性）；
   - 否则 → 调用 `f.ToString(format, CultureInfo.InvariantCulture)`（**不变区域性**，输出与操作系统区域设置无关，保证一致性）；
3. **其它**（`null`、列表、`option` 等非 `IFormattable` 类型）→ 回退到 `stringify`，输出 F# 源码风格。

示例：

```fsharp
formatValue "D5" 42y                  // "00042"
formatValue "D5" 123                  // "00123"
formatValue "N0" 123456               // "123,456"
formatValue "F2" 123.456              // "123.46"
formatValue "0.##" 123.400            // "123.4"
formatValue "yyyy-MM-dd HH:mm:ss" (DateTime(2026, 7, 26, 14, 30, 0))
                                      // "2026-07-26 14:30:00"
formatValue "c" (TimeSpan(1, 2, 3, 4, 5))
                                      // "1.02:03:04.0050000"
formatValue "G" "hello"               // "hello"（字符串原样返回）
formatValue ""  123                   // "123"（同 123.ToString()）
formatValue null (DateTime(2026,7,26,14,30,0))
                                      // 同 x.ToString()
formatValue "G" null                  // "null"（回退到 stringify）
formatValue "G" [1;2;3]               // "[1;2;3]"（回退到 stringify）
formatValue "G" None                  // "None"（回退到 stringify）
```

注意事项：

- 因为字符串是**原样**返回的，`formatValue "G" "null"` 与 `formatValue "G" null` 的输出都是 `"null"`，二者无法从结果区分；
- 由于数值格式化使用不变区域性，`N0` 等千分位格式的输出是 `"123,456"`，不会因中文/英文系统差异变成 `"123.456"`。

---

## 五、依赖与扩展

本模块依赖同一仓库中的两个底层命名空间：

- `FSharp.Idioms.Literals` —— 提供打印器框架（`TypePrinterApp` / `ValuePrinterApp` / `TypePrinters` / `ValuePrinters`），`Literal.stringify*` 是其封装；
- `FSharp.Idioms.Zeros` —— 提供零值框架（`ZeroUtils` / `TryZeros`），`Literal.defaultof*` 是其封装。

打印器与零值规则都以**可替换的规则列表**为参数，因此可以自行实现规则（`Type -> option<...>` 形式的试探函数）来支持自定义类型，或覆盖默认输出风格，而不需要改动本模块。

相关测试用例可作参考：

- `FSharp.Idioms.Tests/Literals/RenderTest.fs`（值打印）
- `FSharp.Idioms.Tests/Literals/TypeRenderTest.fs`（类型打印）
- `FSharp.Idioms.Tests/FormatValueTest.fs`（格式化）
- `FSharp.Idioms.Tests/Zeros/*.fs`（默认值）
