public class RegexBuilder
    {
        private List<object> Inputs;
		private RegexOptions[] Options;

        public RegexBuilder()
        {
            Inputs = new List<object>();
        }

        public RegexBuilder Exactly(string input)
        {
            Inputs.Add(Regex.Escape(input));
            return this;
        }

        public RegexBuilder Exactly(int count)
        {
            Inputs.Add($"{{{count}}}");
            return this;
        }

        public RegexBuilder CharIn(string input)
        {
            Inputs.Add($"[{Regex.Escape(input)}]");
            return this;
        }

        public RegexBuilder CharNotIn(string input)
        {
            Inputs.Add($"[^{Regex.Escape(input)}]");
            return this;
        }

        public RegexBuilder AnyOf(params string[] inputs)
        {
            Inputs.Add($"({string.Join("|", inputs.Select(Regex.Escape))})");
            return this;
        }

        public RegexBuilder Char => Exactly(@".");

        public RegexBuilder Word => Exactly(@"\w");

        public RegexBuilder WordChar => Exactly(@"\w");

        public RegexBuilder WordBoundary => Exactly(@"\b");

        public RegexBuilder Digit => Exactly(@"\d");

        public RegexBuilder Whitespace => Exactly(@"\s");

        public RegexBuilder Letter => Exactly(@"\p{L}");

        public RegexBuilder LowercaseLetter => Exactly(@"\p{Ll}");

        public RegexBuilder UppercaseLetter => Exactly(@"\p{Lu}");

        public RegexBuilder Tab => Exactly(@"\t");

        public RegexBuilder Linefeed => Exactly(@"\n");

        public RegexBuilder CarriageReturn => Exactly(@"\r");

        public RegexBuilder Maybe => new RegexBuilder().Exactly(@"?");

        public RegexBuilder OneOrMore => new RegexBuilder().Exactly(@"+");

        public RegexBuilder As(string groupName)
        {
            Inputs.Add($")(?<{groupName}>");
            return this;
        }

        public RegexBuilder GroupedAs(string groupName)
        {
            Inputs.Add($"(?<{groupName}>");
            return this;
        }

        public RegexBuilder Grouped => Exactly(@"(");

        public RegexBuilder And(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?=.*({builder.GetInputStrings()}))");
        }

        public RegexBuilder Not(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"[^{builder.GetInputStrings()}]");
        }

        public RegexBuilder AndReferenceTo(string groupName)
        {
            Inputs.Add($@"\k<{groupName}>");
            return this;
        }

        public RegexBuilder Or => new RegexBuilder().Exactly(@"|");

        public RegexBuilder After(string expression) => Exactly($@"(?<={expression})");

        public RegexBuilder Before(string expression) => Exactly($@"(?={expression})");

        public RegexBuilder NotAfter(string expression) => Exactly($@"(?<!{expression})");

        public RegexBuilder NotBefore(string expression) => Exactly($@"(?!{expression})");

        public RegexBuilder After(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?<={builder.GetInputStrings()})");
        }

        public RegexBuilder After(RegexBuilder builder)
        {
            return Exactly($"(?<={builder.GetInputStrings()})");
        }

        public RegexBuilder Before(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?={builder.GetInputStrings()})");
        }

        public RegexBuilder Before(RegexBuilder builder)
        {
            return Exactly($"(?={builder.GetInputStrings()})");
        }

        public RegexBuilder NotAfter(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?<!{builder.GetInputStrings()})");
        }

        public RegexBuilder NotAfter(RegexBuilder builder)
        {
            return Exactly($"(?<!{builder.GetInputStrings()})");
        }

        public RegexBuilder NotBefore(Action<RegexBuilder> action)
        {
            var builder = new RegexBuilder();
            action(builder);
            return Exactly($"(?!{builder.GetInputStrings()})");
        }

        public RegexBuilder NotBefore(RegexBuilder builder)
        {
            return Exactly($"(?!{builder.GetInputStrings()})");
        }

        public RegexBuilder Between(int min, int max)
        {
            Inputs.Add($"{{{min},{max}}}");
            return this;
        }

        public RegexBuilder AtLeast(int num)
        {
            Inputs.Add($"{{{num},}}");
            return this;
        }

        public RegexBuilder Max(int max)
        {
            Inputs.Add($"{{0,{max}}}");
            return this;
        }

        public RegexBuilder Any()
        {
            Inputs.Add($"+");
            return this;
        }

        public RegexBuilder Optionally => new RegexBuilder().Exactly(@"?");

        public RegexBuilder LineStart => Exactly(@"^");

        public RegexBuilder LineEnd => Exactly(@"$");

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

        private string GetInputStrings()
        {
            return Regex.Unescape(string.Join("", this.Inputs));
        }
    }