using CalendarSystem;
using Core;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoreBot
{
    public class ChoreService : IChoreService
    {
        private readonly ChoreRepo _choreRepo;

        private readonly IChatClient _chatClient;
        private readonly ICalendarService _calendarService;

        public ChoreService(IChatClient chatClient, ICalendarService calendarService, ChoreRepo choreRepo)
        {
            _chatClient = chatClient;
            _calendarService = calendarService;
            _choreRepo = choreRepo;
        }

        public async Task AddChoreAsync(string userName, string description, ulong channelId)
        {
            var chore = new Chore(userName, description, channelId);
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
    }

    public class ChoreRepo : List<Chore>
    {

    }
}
