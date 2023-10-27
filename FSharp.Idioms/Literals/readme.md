# FSharp.Literals

The package includes several features for literals:

- To print values and types in F# source code style.

- To get default value from type info.

## Getting it over NuGet

```
Install-Package FSharp.Literals
```

## The main usage

### print value

The basic usage is `Literal.stringify<'t> (obj:'t)`:

```F#
open FSharp.Literals
Literal.stringify ([1], [true])
```

result is F# source code, it can be pasted directly into F# files.

```F#
[1], [true]
```

This code uses .NET and F# reflection to walk the structure of values so as to build a formatted representation of the value. You format structural types such as lists and tuples using the syntax of F# source code. Unrecognized values are formatted by calling the F# `sprintf "%A"` method for these values.

The overloads function as same output as `Literal.stringify` is:

```F#
open FSharp.Literals
Literal.stringifyDynamic typeof<int list * bool list> ([1], [true])
```


### print type

The basic usage is `Literal.printTypeDynamic (ty:Type)`:

```F#
open FSharp.Literals

let ty = typeof<(string*int)*(float*bool)>
let y = Literal.printTypeDynamic ty
Should.equal y "(string*int)*(float*bool)"

```

You can also use shortcut methods same as this method:

```F#
let y = Literal.printType<(string*int)*(float*bool)>
Should.equal y "(string*int)*(float*bool)"
```

to print F# types using the syntax of F# source code. In order to please the C#-programmers, it's all this `list<int>` style instead of `int list`. You can also re-implement `TypePrinter` interfaces to custom printed result in your styles.

> **Note**: `Should.equal` in library `FSharp.xUnit`.

### default value

You often need to know the default values for a certain type. You can use function `Literal.defaultValueDynamic`:

```F#
let x = typeof<char>
let y = Literal.defaultValueDynamic x :?> char
should.equal y '\u0000'
```

You can also use shortcut methods same as this method:

```F#
let y = Literal.defaultValue<char>
should.equal y '\u0000'
```

It can resolve any type of default value, the system defines common composite types, even if the system is not defined types that you need, you can supplement the composite types that you need, the library provides a recursive framework.