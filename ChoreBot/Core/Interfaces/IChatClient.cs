using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChatClient
    {
        Task SendMessageAsync(string assignee, string description, ulong channelId);
        Task SendChoreMessageAsync(Chore choreToRemind);
    }
}
