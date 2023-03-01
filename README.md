# Regex Builder

Similar to Magic-Regexp, allows for a more intuitive way to build regexes.

Example 1:

```csharp
var regex = new RegexBuilder()
    .LineStart          //Ensure that the string must start with the following expression, in this case, S, T, F, G or M
    .CharIn("STFGM")    // S, T, F, G, or M
    .Digit.Exactly(7)	// Exactly 7 digits. You can also do something like this: And(x => x.Digit.Exactly(7).Letter.Exactly(1))
    .Letter.Exactly(1)  // Exactly 1 letter, or to be more specific you can do CharIn(): 
    //.CharIn("A-Za-z") // Or you can do it this way to compare a single character from, don't need to call .Exactly(1)
    .LineEnd            // Ensure that the string must end with the preceding expression, in this case, a letter/character
    .ToRegex(RegexOptions.IgnoreCase, RegexOptions.Singleline);	// Other Options
var isMatchRegex = regex.IsMatch(exampleString);

// Regex String : ^[STFGM]\d{7}[A-Za-z]$
```

Example 2:

```csharp
var isMatch = testString.IsMatch(regex => regex
    .LineStart
    .CharIn("STFGM")
    .Digit.Exactly(7)
    .Letter.Exactly(1)
    .ToRegex(RegexOptions.IgnoreCase)
    .LineEnd
);

// Regex String : ^[STFGM]\d{7}[A-Za-z]$
```
