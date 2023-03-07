using System.Collections.Generic;
using System.Linq;

namespace System.Text.RegularExpressions
{
    /// <summary>
    /// A class for building regular expressions in a more readable and maintainable way.
    /// </summary>
    public class RegexBuilder
    {
        /// <summary>
        /// List of Inputs in the order of the expressions being built.
        /// </summary>
        private List<object> Inputs;

        private RegexOptions[] Options;

        public RegexBuilder()
        {
            Inputs = new List<object>();
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a carriage return (\r).
        /// </summary>
        public RegexBuilder CarriageReturn => Exactly(@"\r");

        /// <summary>
        /// Returns a RegexBuilder object that represents any character except newline (.).
        /// </summary>
        public RegexBuilder Char => Exactly(@".");

        /// <summary>
        /// Returns a RegexBuilder object that represents a digit (\d).
        /// </summary>
        public RegexBuilder Digit => Exactly(@"\d");

        /// <summary>
        /// Returns a RegexBuilder object that represents a capturing group (`(`).
        /// </summary>
        public RegexBuilder Grouped => Exactly(@"(");

        /// <summary>
        /// object that represents any Unicode letter character (\p{L}).
        /// </summary>
        public RegexBuilder Letter => Exactly(@"\p{L}");

        /// <summary>
        /// Returns a RegexBuilder object that represents the end of a line ($).
        /// </summary>
        public RegexBuilder LineEnd => Exactly(@"$");

        /// <summary>
        /// Returns a RegexBuilder object that represents a linefeed (\n).
        /// </summary>
        public RegexBuilder Linefeed => Exactly(@"\n");

        /// <summary>
        /// Returns a RegexBuilder object that represents the start of a line (^).
        /// </summary>
        public RegexBuilder LineStart => Exactly(@"^");

        /// <summary>
        /// Returns a RegexBuilder object that represents a lowercase Unicode letter character (\p{Ll}).
        /// </summary>
        public RegexBuilder LowercaseLetter => Exactly(@"\p{Ll}");

        /// <summary>
        /// Returns a RegexBuilder object that represents zero or one occurrences of the previous element (?).
        /// </summary>
        public RegexBuilder Maybe => Exactly(@"?");

        /// <summary>
        /// Returns a RegexBuilder object that represents one or more occurrences of the previous element (+).
        /// </summary>
        public RegexBuilder OneOrMore => Exactly(@"+");

        /// <summary>
        /// Returns a RegexBuilder object that represents zero or one occurrences of the previous element (?).
        /// </summary>
        public RegexBuilder Optionally => Exactly(@"?");

        /// <summary>
        /// Returns a RegexBuilder object that represents an alternation (|).
        /// </summary>
        public RegexBuilder Or => Exactly(@"|");

        /// <summary>
        /// Returns a RegexBuilder object that represents a tab character (\t).
        /// </summary>
        public RegexBuilder Tab => Exactly(@"\t");

        /// <summary>
        /// Returns a RegexBuilder object that represents an uppercase Unicode letter character (\p{Lu}).
        /// </summary>
        public RegexBuilder UppercaseLetter => Exactly(@"\p{Lu}");

        /// <summary>
        /// Returns a RegexBuilder object that represents any whitespace character (\s).
        /// </summary>
        public RegexBuilder Whitespace => Exactly(@"\s");

        /// <summary>
        /// Returns a RegexBuilder object that represents any word character (\w).
        /// </summary>
        public RegexBuilder Word => Exactly(@"\w");

        /// <summary>
        /// Returns a RegexBuilder object that represents a word boundary (\b).
        /// </summary>
        public RegexBuilder WordBoundary => Exactly(@"\b");

        /// <summary>
        /// Returns a RegexBuilder object that represents any word character (\w).
        /// </summary>
        public RegexBuilder WordChar => Exactly(@"\w");

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookbehind assertion for expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RegexBuilder After(string expression) => Exactly($@"(?<={expression})");

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookbehind assertion for a subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder After(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?<={builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookbehind assertion for the regular expression built using builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public RegexBuilder After(RegexBuilder builder)
        {
            return Exactly($"(?<={builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookahead assertion for a subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder And(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?=.*({builder.GetInputStrings()}))");
        }

        /// <summary>
        /// Adds a reference to a named group in the pattern (\k<groupName>).
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public RegexBuilder AndReferenceTo(string groupName)
        {
            Inputs.Add($@"\k<{groupName}>");
            return this;
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents one or more occurrences of any character (+).
        /// </summary>
        /// <returns></returns>
        public RegexBuilder Any()
        {
            Inputs.Add($"+");
            return this;
        }

        /// <summary>
        /// object that represents a character class containing the specified inputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public RegexBuilder AnyOf(params string[] inputs)
        {
            Inputs.Add($"({string.Join("|", inputs.Select(Regex.Escape))})");
            return this;
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a capturing group with the specified groupName.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public RegexBuilder As(string groupName)
        {
            Inputs.Add($")(?<{groupName}>");
            return this;
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents the previous element occurring at least num times ({num,}).
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public RegexBuilder AtLeast(int num)
        {
            Inputs.Add($"{{{num},}}");
            return this;
        }

        /// <summary>
        /// Represents a negative lookbehind assertion for expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RegexBuilder Before(string expression) => Exactly($@"(?={expression})");

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookahead assertion for a subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder Before(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?={builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a positive lookahead assertion for the regular expression built using builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public RegexBuilder Before(RegexBuilder builder)
        {
            return Exactly($"(?={builder.GetInputStrings()})");
        }

        /// <summary>
        /// Adds a quantifier to match the previous input between min and max times.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public RegexBuilder Between(int min, int max)
        {
            Inputs.Add($"{{{min},{max}}}");
            return this;
        }

        /// <summary>
        /// Adds a character set to match any character in input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public RegexBuilder CharIn(string input)
        {
            Inputs.Add($"[{Regex.Escape(input)}]");
            return this;
        }

        /// <summary>
        /// Adds a character set to match any character not in input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public RegexBuilder CharNotIn(string input)
        {
            Inputs.Add($"[^{Regex.Escape(input)}]");
            return this;
        }

        /// <summary>
        /// Adds an exact match for input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public RegexBuilder Exactly(string input)
        {
            Inputs.Add(Regex.Escape(input));
            return this;
        }

        /// <summary>
        /// Adds a quantifier to match the previous input exactly count times.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public RegexBuilder Exactly(int count)
        {
            Inputs.Add($"{{{count}}}");
            return this;
        }

        /// <summary>
        /// Adds a capturing group with the specified groupName.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public RegexBuilder GroupedAs(string groupName)
        {
            Inputs.Add($"(?<{groupName}>");
            return this;
        }

        /// <summary>
        /// Adds a quantifier to match the previous input up to max times.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public RegexBuilder Max(int max)
        {
            Inputs.Add($"{{0,{max}}}");
            return this;
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative character set that matches any character not in the subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder Not(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"[^{builder.GetInputStrings()}]");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookahead assertion for expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RegexBuilder NotAfter(string expression) => Exactly($@"(?<!{expression})");

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookahead assertion for a subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder NotAfter(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?<!{builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookahead assertion for the regular expression built using builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public RegexBuilder NotAfter(RegexBuilder builder)
        {
            return Exactly($"(?<!{builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookbehind assertion for expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RegexBuilder NotBefore(string expression) => Exactly($@"(?!{expression})");

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookbehind assertion for a subexpression built using the provided action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RegexBuilder NotBefore(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?!{builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a RegexBuilder object that represents a negative lookbehind assertion for the regular expression built using builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public RegexBuilder NotBefore(RegexBuilder builder)
        {
            return Exactly($"(?!{builder.GetInputStrings()})");
        }

        /// <summary>
        /// Returns a Regex object compiled from the regular expression pattern built with the RegexBuilder instance.
        /// The optional options parameter specifies one or more RegexOptions enumeration values to use when compiling the regular expression.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Regex ToRegex(params RegexOptions[] options)
        {
            if (options.Count() == 0 && this.Options.Any())
            {
                options = Options;
            }
            else
            {
                this.Options = options;
            }

            var pattern = Regex.Unescape($"{string.Join("", Inputs)}");
            Optimise(pattern);
            return new Regex(pattern, (RegexOptions)options.Aggregate(0, (acc, option) => acc | (int)option));
        }

        /// <summary>
        /// Returns a string that represents the regular expression pattern built with the RegexBuilder instance.
        /// </summary>
        /// <returns></returns>
        private string GetInputStrings()
        {
            return Regex.Unescape(string.Join("", this.Inputs));
        }

        /// <summary>
        /// Optimizes the specified pattern regular expression string by simplifying character sets,
        /// unrolling quantifiers, and reordering alternation groups. This method is used internally
        /// by the RegexBuilder class.
        /// </summary>
        /// <param name="pattern"></param>
        private void Optimise(string pattern)
        {
            // Simplify character sets
            pattern = Regex.Replace(pattern, @"\[(?<chars>[^\[\]])\]", m =>
            {
                var chars = m.Groups["chars"].Value;
                return "[" + string.Concat(chars.OrderBy(c => c).Distinct().Select((c, i) =>
                    i > 0 && c == chars[i - 1] + 1 ? "" : c.ToString() + (i == chars.Length - 1 ? "" : "-")));
            });

            // Unroll quantifiers
            pattern = Regex.Replace(pattern, @"(?<=\\?.){(\d+)}", m =>
            {
                var count = int.Parse(m.Groups[1].Value);
                return string.Concat(Enumerable.Repeat(m.Value.Substring(0, m.Value.Length - 3), count));
            });

            // Reorder alternation
            pattern = Regex.Replace(pattern, @"\((?:[^\(\)\|]*\|)+[^\(\)\|]*\)", m =>
            {
                var alternatives = m.Value.Substring(1, m.Value.Length - 2).Split('|');
                var reordered = alternatives.OrderByDescending(a => a.Length).ToArray();
                return "(" + string.Join("|", reordered) + ")";
            });
        }
    }
}