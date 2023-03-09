using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    internal class Chore
    {
        public string Assignee { get; set; }

        public Chore(string assignee, string description)
        {
            Assignee = assignee;
            Description = description;
        }

        public string Description { get; set; }
    }
}
