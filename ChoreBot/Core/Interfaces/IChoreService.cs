using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChoreService
    {
        Task AddChoreAsync(string assignee, string description, ulong channelId);
        Task RemindAsync(Guid choreToSchedule);
    }
}
