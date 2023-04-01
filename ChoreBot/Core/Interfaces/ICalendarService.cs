using Core;

namespace Core
{
    public interface ICalendarService
    {
        Task ScheduleChoreAsync(Chore choreToSchedule);
    }
}