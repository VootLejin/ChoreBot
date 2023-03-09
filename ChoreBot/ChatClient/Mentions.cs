using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatClient
{
    internal static class Mentions
    {

        private static Regex MentionSyntax = new Regex("^<@.*>");


        public static bool IsMention(string userTarget)
        {
            return MentionSyntax.IsMatch(userTarget);
        }
    }
}
