public static class RegexExtensions
{
    public static bool IsMatch(this string input, Action<RegexBuilder> regexBuilder)
    {
        var regexBuilder = new RegexBuilder();
        regexBuilder(regexBuilder);
        var regex = regexBuilder.ToRegex();
        return regex.IsMatch(input);
    }
}