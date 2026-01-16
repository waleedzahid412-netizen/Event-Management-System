using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.ViewModels;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Event_Management_System.Repositories.Implementations
{
    public class AdminAnalyticsRepository :IAdminAnalyticsRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminAnalyticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: Determine start date based on filter
        private DateTime GetStartDate(DateFilterType filter)
        {
            return filter switch
            {
                DateFilterType.Week => DateTime.Now.AddDays(-7),
                DateFilterType.Month => DateTime.Now.AddMonths(-1),
                DateFilterType.Year => DateTime.Now.AddYears(-1),
                _ => DateTime.Now.AddYears(-1)
            };
        }

        public async Task<Dictionary<string, int>> GetEventsByStatusAsync(DateFilterType filter)
        {
            var startDate = GetStartDate(filter);

            var data = await _context.Events
                .Where(e => e.StartDate >= startDate)
                .GroupBy(e => e.Status)          // ✅ enum (SQL-safe)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return data.ToDictionary(
                x => x.Status.ToString(),        // ✅ happens in memory
                x => x.Count
            );
        }

        public async Task<Dictionary<string, int>> GetRegistrationsPerEventAsync(DateFilterType filter)
        {
            var startDate = GetStartDate(filter);

            return await _context.Events
                .Where(e => e.StartDate >= startDate)
                .Select(e => new { e.Title, Count = e.Registrations.Count })
                .ToDictionaryAsync(e => e.Title, e => e.Count);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueOverTimeAsync(DateFilterType filter)
        {
            var startDate = GetStartDate(filter);

            var registrations = await _context.Registrations
                .Include(r => r.Payment)
                .Where(r =>
                    r.RegisteredOn >= startDate &&
                    r.Payment != null &&
                    r.Payment.Status == "Completed"
                )
                .ToListAsync();

            return registrations
                .GroupBy(r => filter == DateFilterType.Year
                    ? r.RegisteredOn.ToString("MMM")
                    : r.RegisteredOn.ToString("dd MMM"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(r => r.Payment!.Amount)
                );
        }

        public async Task<Dictionary<string, int>> GetUpcomingEventsOverTimeAsync(DateFilterType filter)
        {
            var startDate = GetStartDate(filter);

            var upcomingEvents = await _context.Events
                .Where(e => e.StartDate >= startDate)
                .ToListAsync();

            return upcomingEvents
                .GroupBy(e => filter == DateFilterType.Year
                    ? e.StartDate.ToString("MMM")
                    : e.StartDate.ToString("dd MMM"))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, decimal>> GetAvgTicketPriceOverTimeAsync(DateFilterType filter)
        {
            var startDate = GetStartDate(filter);

            var registrations = await _context.Registrations
                .Include(r => r.Payment)
                .Where(r =>
                    r.RegisteredOn >= startDate &&
                    r.Payment != null &&
                    r.Payment.Status == "Completed"
                )
                .ToListAsync();

            return registrations
                .GroupBy(r => filter == DateFilterType.Year
                    ? r.RegisteredOn.ToString("MMM")
                    : r.RegisteredOn.ToString("dd MMM"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Any()
                        ? g.Average(r => r.Payment!.Amount)
                        : 0m
                );
        }

    }
}
