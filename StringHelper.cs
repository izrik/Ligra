using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetaphysicsIndustries.Ligra
{
    public static class StringHelper
    {
        public static string PrefixLines(this string s, string prefix)
        {
            var sb = new StringBuilder();
            foreach (var line in s.SplitLines())
            {
                if (!string.IsNullOrEmpty(line))
                {
                    sb.AppendFormat("{0}{1}", prefix, line);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static IEnumerable<string> SplitLines(this string s)
        {
            using (var reader = new StringReader(s))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
