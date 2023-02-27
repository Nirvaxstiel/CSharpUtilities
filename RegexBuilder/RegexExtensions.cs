public static class RegexExtensions
{
    public static bool IsMatch(this string input, Action<RegexBuilder> regexBuilder)
    {
        var builder = new RegexBuilder();
        regexBuilder(builder);
        var regex = builder.ToRegex();
        return regex.IsMatch(input);
    }
}