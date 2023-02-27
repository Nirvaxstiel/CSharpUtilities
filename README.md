# Regex Builder

Similar to Magic-Regexp, allows for a more intuitive way to build regexes.

Sample 1:

```csharp
var testString = "T1231234T";
var regexString = new RegexBuilder()
    CharIn("STFGM")
    .Digit.Exactly(7)
    .Letter.Exactly(1)
    .ToRegex(RegexOptions.IgnoreCase);
var isMatch = regexString.IsMatch(testString);
```

Sample 2:

```csharp
var isMatch = testString.IsMatch(regex => regex.CharIn("STFGM")
    .Digit.Exactly(7)
    .Letter.Exactly(1)
    .ToRegex(RegexOptions.IgnoreCase)
);
```
