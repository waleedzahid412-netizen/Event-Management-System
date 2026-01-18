using Event_Management_System.Models.Enums;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Event_Management_System.Services.Implementations
{
    public class EventStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;


        public EventStatusBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _eventRepository = scope.ServiceProvider
                .GetRequiredService<IEventRepository>();



                var expiredEvents = await _eventRepository.GetCompletedEventsAsync();


                foreach (var e in expiredEvents)
                {
                    e.Status = EventStatus.Completed;
                }

                if (expiredEvents.Any())
                    await _eventRepository.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
