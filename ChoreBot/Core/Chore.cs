using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Chore
    {
        public Chore(string assignee, string description, ulong channelId)
        {
            Assignee = assignee;
            Description = description;
            Id = Guid.NewGuid();
            ChannelId = channelId;
        }

        public Guid Id { get; }

        public string Assignee { get; set; }

        public string Description { get; set; }

        public ulong ChannelId { get; private set; }
    }
}
