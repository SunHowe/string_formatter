# C# string formatter with custom rules

## usage

see unit tests code in `Tests.cs`.

### 1. simple format (like c# string.Format)

code:
```c#
StringEx.Format("{0}", "argument1")
```

result:
```text
argument1
```

### 2. custom rule format without parameters

```c#
// only register once
StringEx.RegisterFormatFunc("name", () => "PlayerNameA");

// after register
StringEx.Format("{name}")
```

result:
```text
PlayerNameA
```

### 3. custom rule format with parameters

```c#
// only register once
StringEx.RegisterFormatFunc("i18n", (index, parameters) => 
    {
        // TOOD load text from i18n config
        return $"i18n_content_({parameters[index]})";
    });

// after register
StringEx.Format("{0:i18n}", 1001)
```

result:
```text
i18n_content_1001
```

### 4. different rules in different context

```c#
var formatter1 = new StringFormatter.StringFormatter();
formatter1.RegisterFormatFunc("name", () => "name in context1");
            
var formatter2 = new StringFormatter.StringFormatter();
formatter2.RegisterFormatFunc("name", () => "name in context2");

// after register
formatter1.Format("{name}");
formatter2.Format("{name}");
```

result:
```text
name in context1
name in context2
```

