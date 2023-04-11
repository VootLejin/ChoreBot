using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IChoreService
    {
        Task AddChoreAsync(string userName, string description, ulong channelId, DateTimeOffset dueTime);

        Task RemindAsync(Guid choreToSchedule);

        Task EndChoreAsync(string user, ulong channelId);
        event EventHandler<ChoreEndDetails> ChoreEnded;
        Task RemoveChore(Chore choreToRemove);
        Task<Chore> GetChoreAsync(Guid choreId);
    }
}
