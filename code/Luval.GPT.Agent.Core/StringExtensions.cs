using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ToParagraphs(this string str)
        {
            return Regex.Split(str, @"(\n|\.|(\n\n)|(\?\n)|(!\n))").Where(i => i.Length > 1);
        }
    }
}
