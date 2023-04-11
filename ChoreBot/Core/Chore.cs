using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Chore
    {
        public Chore(string assignee, string description, ulong channelId, DateTimeOffset dueTime)
        {
            Assignee = assignee;
            Description = description;
            Id = Guid.NewGuid();
            ChannelId = channelId;
            DueTime = dueTime;
        }

        public Guid Id { get; }

        public string Assignee { get; set; }

        public string Description { get; set; }

        public ulong ChannelId { get; private set; }

        public DateTimeOffset DueTime { get; set; }
        public string JobId { get; set; }
        public bool Complete { get; set; }
        public ulong? MessageId { get; set; }
    }
}
