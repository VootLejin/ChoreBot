using Core;
using Core.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CalendarSystem
{
    public class CalendarService : ICalendarService, IDisposable
    {
        private readonly Lazy<IChoreService> _choreService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly BackgroundJobServer _backgroundJobServer;
        private bool disposedValue;

        public CalendarService(Lazy<IChoreService> choreService, IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobServer = new BackgroundJobServer();
            _choreService = choreService;
            _backgroundJobClient = backgroundJobClient;
        }


        public async Task ScheduleChoreAsync(Chore choreToSchedule)
        {
            var id = choreToSchedule.Id;
            //_backgroundJobClient.Enqueue(() => _choreService.Value.RemindAsync(id));
            var jobId = _backgroundJobClient.Schedule(() => _choreService.Value.RemindAsync(id), choreToSchedule.DueTime);
            choreToSchedule.JobId = jobId;
            _backgroundJobClient.ContinueJobWith(jobId, () => ScheduleChoreReminderAsync(id, TimeSpan.FromMinutes(1)));
        }


        public async Task ScheduleChoreReminderAsync(Guid choreId, TimeSpan reminderInterval)
        {
            var choreToSchedule = await _choreService.Value.GetChoreAsync(choreId);
            if (choreToSchedule == null)
            {
                return;
            }

            if (choreToSchedule.Complete)
            {
                await _choreService.Value.RemoveChore(choreToSchedule);
                return;
            }
            var id = choreToSchedule.Id;
            //_backgroundJobClient.Enqueue(() => _choreService.Value.RemindAsync(id));
            var jobId = _backgroundJobClient.Schedule(() => _choreService.Value.RemindAsync(id), reminderInterval);
            choreToSchedule.JobId = jobId;
            _backgroundJobClient.ContinueJobWith(jobId, () => ScheduleChoreReminderAsync(choreId, reminderInterval));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _backgroundJobServer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CalendarService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

    public static class SerivceCollector
    {
        public static IServiceCollection ConfigureCalanderSystemService(this IServiceCollection serviceCollection)
        {

            serviceCollection.AddHangfire(x => x.UseSqlServerStorage("Server=localhost\\SQLEXPRESS;Database=Hangfire;Trusted_Connection=True;"));
            serviceCollection.AddHangfireServer();

            serviceCollection.AddSingleton<ICalendarService, CalendarService>();

            return serviceCollection;

        }
    }
}