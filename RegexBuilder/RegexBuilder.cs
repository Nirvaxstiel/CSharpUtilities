public class RegexBuilder : IDisposable
{
	private readonly StringBuilder _pattern;
    private RegexOptions _options;
    
	public RegexBuilder()
    {
        _pattern = new StringBuilder();
        _options = RegexOptions.None;
    }
	
	public RegexBuilder(string pattern, RegexOptions options= RegexOptions.None)
	{
		_pattern = new StringBuilder(pattern);
		_options = options;
	}
	
    public RegexBuilder StartOfLine() => Append("^");
    public RegexBuilder EndOfLine() => Append("$");
    public RegexBuilder AnyCharacter() => Append(".");
    public RegexBuilder AnyOf(params char[] chars) => Append($"[{string.Join("", chars)}]");
    public RegexBuilder AnyOf(params string[] inputs) => Append($"({string.Join("|", inputs.Select(Regex.Escape))})");
	public RegexBuilder NoneOf(params string[] chars) => Append($"[^{string.Join("", chars)}]");
	public RegexBuilder Exactly(int n) => Append($"{{{n}}}");
    public RegexBuilder Exactly(string text) => Append(Regex.Escape(text));
    public RegexBuilder ZeroOrMore() => Append("*");
    public RegexBuilder OneOrMore() => Append("+");
    public RegexBuilder ZeroOrOne() => Append("?");
    public RegexBuilder And(params RegexBuilder[] regexBuilders) => Append(string.Join("", regexBuilders.Select(x => x._pattern)));
    public RegexBuilder Not() => Append("^");
    public RegexBuilder Or(params RegexBuilder[] regexBuilders) => Append($"({string.Join("|", regexBuilders.Select(x => x._pattern))})");
    public RegexBuilder Between(int min, int max) => Append($"{{{min},{max}}}");
    public RegexBuilder AtLeast(int min) => Append($"{{{min},}}");
    public RegexBuilder AtMost(int max) => Append($"{{0,{max}}}");
    public RegexBuilder Word() => Append("\\w+");
    public RegexBuilder Digit() => Append("\\d");
    public RegexBuilder Whitespace() => Append("\\s");
    public RegexBuilder Tab() => Append("\\t");
    public RegexBuilder Newline() => Append("\\n");
    public RegexBuilder Linebreak() => Append("\\r\\n|\\r|\\n");
    public RegexBuilder IgnoreCase() => Insert(0, "(?i)");
    public RegexBuilder Multiline() => Insert(0, "(?m)");
    public RegexBuilder PositiveLookahead(string text) => Append($"(?={text})");
	public RegexBuilder PositiveLookahead(RegexBuilder regexBuilder) => Append($"(?={regexBuilder._pattern})");
    public RegexBuilder PositiveLookbehind(string text) => Append($"(?<={text})");
	public RegexBuilder PositiveLookbehind(RegexBuilder regexBuilder) => Append($"(?<={regexBuilder._pattern})");	
	public RegexBuilder NegativeLookahead(char c) => Append($"(?!.*[{Regex.Escape(c.ToString())}])");
    public RegexBuilder NegativeLookahead(string text) => Append($"(?!{text})");
	public RegexBuilder NegativeLookbehind(string pattern) => Append($"(?<!{pattern})");
    public RegexBuilder NegativeLookbehind(RegexBuilder regexBuilder) => Append($"(?<!{regexBuilder._pattern})");		
    public RegexBuilder Optionally() => Append("?");
    public RegexBuilder Mandatory() => Append("+");
    public RegexBuilder CharIn(string str) => Append($"[{Regex.Escape(str)}]");
    public RegexBuilder CharNotIn(string str) => Append($"[^{Regex.Escape(str)}]");
    public RegexBuilder Char() => Append(".");
    public RegexBuilder WordChar() => Append("\\w");
    public RegexBuilder WordBoundary() => Append("\\b");
    public RegexBuilder Letter() => Append("[A-Za-z]");
    public RegexBuilder LowercaseLetter() => Append("[a-z]");
    public RegexBuilder UppercaseLetter() => Append("[A-Z]");

    private RegexBuilder Append(string str)
    {
        _pattern.Append(str);
        return this;
    }

    private RegexBuilder Insert(int index, string str)
    {
        _pattern.Insert(index, str);
        return this;
    }
	
	public RegexBuilder WithOptions(params RegexOptions[] options)
	{
		foreach (var option in options)
        {
            this._options |= option;
        }
        return this;
	}

	public RegexBuilder RemoveOptions(params RegexOptions[] options)
	{
		foreach (var option in options)
        {
            this._options &= ~option;
        }
        return this;
	}
	
	public RegexBuilder Optimise()
	{
	    string pattern = this._pattern.ToString();
	    RemoveUnnecessaryCharacters(pattern);
	    MergeCharacterClasses(pattern);
	    ReorderAlternations(pattern);
	    SimplifyQuantifiers(pattern);
	    RemoveUnnecessaryGroups(pattern);
	    return new RegexBuilder(pattern, this._options);
	}

	private string Optimise(string pattern)
	{
	    RemoveUnnecessaryCharacters(pattern);
	    MergeCharacterClasses(pattern);
	    ReorderAlternations(pattern);
	    SimplifyQuantifiers(pattern);
	    RemoveUnnecessaryGroups(pattern);
	    return pattern;
	}

	private void RemoveUnnecessaryCharacters(string pattern)
	{
		pattern = Regex.Replace(pattern, @"(?<!\\)\\(?=\\)", "");
		pattern = Regex.Replace(pattern, @"(?<!\\)\\(?=\s)", "");
		pattern = Regex.Replace(pattern, @"(?<=\s)\\(?!$)", "");
	}

	private void MergeCharacterClasses(string pattern)
	{
		pattern = Regex.Replace(pattern, @"\[\s*(\\.|[^\]\\])+\s*\]", match =>
		{
			string merged = "";
			bool negate = false;
			foreach (char c in match.Value)
			{
				if (c == '[')
				{
					merged += c;
				}
				else if (c == '^' && merged.Length == 1)
				{
					negate = true;
				}
				else if (c == ']')
				{
					merged += c;
					break;
				}
				else
				{
					merged += c;
				}
			}
			string patternPart = merged.Substring(1, merged.Length - 2);
			if (negate)
			{
				patternPart = "^" + patternPart;
			}
			patternPart = Regex.Escape(patternPart);
			return "[" + patternPart + "]";
		});
	}

	private void ReorderAlternations(string pattern)
	{
		pattern = Regex.Replace(pattern, @"\|(?<insideBrackets>[^\[\]]*\])+(?<afterBrackets>[^\|]*)", match =>
		{
			string insideBrackets = match.Groups["insideBrackets"].Value;
			string afterBrackets = match.Groups["afterBrackets"].Value;
			string reordered = "|" + afterBrackets + insideBrackets;
			return reordered;
		});
	}

	private void SimplifyQuantifiers(string pattern)
	{
		pattern = Regex.Replace(pattern, @"(\w)(?<!\(\?)(\*)\+", "$1*");
		pattern = Regex.Replace(pattern, @"(\w)(?<!\(\?)(\+){2,}", "$1$2");
		pattern = Regex.Replace(pattern, @"(\w)(?<!\(\?)(\*){2,}", "$1*");
	}

	private void RemoveUnnecessaryGroups(string pattern)
	{
		pattern = Regex.Replace(pattern, @"(?<!\\)\((?<group>[^?].*?)\)(?!\*)", "${group}");
		pattern = Regex.Replace(pattern, @"\(\?<.*?>", "(");
	}

    public Regex ToRegex()
    {
        return new Regex(_pattern.ToString());
    }

    public void Dispose()
    {
        //_pattern.Clear();
    }
}