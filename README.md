# Regex Builder

Similar to Magic-Regexp, allows for a more intuitive way to build regexes.

Sample:

```
var testString = "S1231234d";
var isMatch = testString.IsMatch(regex=>
    regex.StartOfLine()
    //.NegativeLookbehind(".") //If you want to be more explicit, but this is more expensive.
    //.AnyOf(new char[] {'T','S',}).WithOptions(RegexOptions.IgnoreCase) 
    .AnyOf("TSGMF", RegexOptions.IgnoreCase) //if you want to use ToRegexString() and print a usable string and wish to keep upper and lower case set for some reason.
    .Digit().Exactly(7)
    .Char().Exactly(1)
    .EndOfLine()
    //.NegativeLookahead(".") //If you want to be more explicit, but this is more expensive.
    .Optimise()
    .ToRegex()
);
Console.WriteLine(isMatch);
```
