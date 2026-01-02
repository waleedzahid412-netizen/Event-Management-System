using Event_Management_System.Models;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Repositories.Implementations
{
    public class OrganizerApplicationRepository : IOrganizerApplicationRepository
    {
        public readonly ApplicationDbContext _context;
        public OrganizerApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async  Task AddApplicationAsync(OrganizerApplication obj)
        {
           await  _context.OrganizerApplications.AddAsync(obj);
        }

        public async Task<bool> CheckIfOrganizerApplicationExistAsync(int userid)
        {
            return await _context.OrganizerApplications.AnyAsync(o => o.UserId == userid && o.Status==ApplicationStatus.Pending);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
