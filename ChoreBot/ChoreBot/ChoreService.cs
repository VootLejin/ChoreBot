using CalendarSystem;
using Core;
using Core.Interfaces;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChoreBot
{
    public class ChoreService : IChoreService
    {
        private readonly ChoreRepo _choreRepo;

        private readonly IChatClient _chatClient;
        private readonly ICalendarService _calendarService;

        public event EventHandler<ChoreEndDetails> ChoreEnded;

        public ChoreService(IChatClient chatClient, ICalendarService calendarService, ChoreRepo choreRepo)
        {
            _chatClient = chatClient;
            _calendarService = calendarService;
            _choreRepo = choreRepo;
        }

        public async Task AddChoreAsync(string userName, string description, ulong channelId, DateTimeOffset dueTime)
        {
            var chore = new Chore(userName, description, channelId, dueTime);
            _choreRepo.Add(chore);
            await _calendarService.ScheduleChoreAsync(chore);

            Console.WriteLine($"Chore Added: {chore.Assignee} -> {chore.Description}");
        }

        public async Task RemindAsync(Guid choreIdToRemind)
        {
            var choreToRemind = _choreRepo.Find(c => c.Id == choreIdToRemind);
            if (choreToRemind == null)
            {
                return;
            }
            await _chatClient.SendMessageAsync(choreToRemind.Assignee, choreToRemind.Description, choreToRemind.ChannelId);
        }

        public async Task EndChoreAsync(string user, ulong channelId)
        {
            var choreToRemind = _choreRepo.Find(c => c.Assignee == user && !c.Complete);
            if (choreToRemind == null)
            {
                return;
            }
            OnChoreEnded(new ChoreEndDetails { User = user, ChannelId = channelId});
            choreToRemind.Complete = true;
        }

        protected virtual void OnChoreEnded(ChoreEndDetails choreEndDetails)
        {
            ChoreEnded?.Invoke(this, choreEndDetails);
        }

        public async Task RemoveChore(Chore choreToRemove)
        {
            _choreRepo?.Remove(choreToRemove);
        }

        public async Task<Chore> GetChoreAsync(Guid choreId)
        {
            return _choreRepo?.Find(c => c.Id == choreId);
        }
    }

    public class ChoreRepo : List<Chore>
    {

    }
}
