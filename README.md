# Regex Builder

Similar to Magic-Regexp, allows for a more intuitive way to build regexes.

Sample:

```
	var testString = "S1231234d";
	var isMatch = testString.IsMatch(builder=>
		builder.StartOfLine()
		.NegativeLookbehind(".")
		.AnyOf(new char[] {'T','S',}).WithOptions(RegexOptions.IgnoreCase)
		.Digit().Exactly(7)
		.Char().Exactly(1)
		.EndOfLine()
		.NegativeLookahead(".") // negative lookahead for any additional characters after the required ending character
		.Optimise()
		.ToRegex()
	);
	Console.WriteLine(isMatch);
```
