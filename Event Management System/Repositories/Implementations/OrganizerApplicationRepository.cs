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

        public async Task<bool> checkIfApplicationExistButPaymentPending(int userid)
        {
            return await _context.OrganizerApplications
                .AnyAsync(oa => oa.UserId == userid && !oa.IsPaymentCompleted);

            
        }

        public async Task<bool> CheckIfOrganizerApplicationExistAsync(int userid)
        {
            return await _context.OrganizerApplications.AnyAsync(o => o.UserId == userid && o.Status==ApplicationStatus.Pending);
        }

        public async Task<int> GetApplicationIdOfUser(int userid)
        {
            return await _context.OrganizerApplications
            .Where(u => u.UserId == userid)
            .Select(u => u.OrganizerApplicationId)
            .FirstAsync();
        }

        public async Task<OrganizerApplication> GetApprovedApplications()
        {
            return await _context.OrganizerApplications
                .FirstAsync(o => o.Status == ApplicationStatus.Approved);
        }

        public async Task<OrganizerApplication?> GetOrganizerApplicationByIdAsync(int applicationid)
        {
            return await _context.OrganizerApplications.Include(og=>og.User).
                FirstOrDefaultAsync(og => og.OrganizerApplicationId == applicationid);
        }

        public async Task<List<OrganizerApplication>> GetPendingOrganizerApplications()
        {
            return await _context.OrganizerApplications
                .Include(o => o.User)
                .Where(o => o.Status ==ApplicationStatus.Pending)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> TotalOrganizerPendingRequestAsync()
        {
            return await _context.OrganizerApplications.CountAsync(o => o.Status ==ApplicationStatus.Pending);
        }

        public async Task<List<OrganizerApplication>> GetApprovedOrganizerApplications()
        {
            return await _context.OrganizerApplications
                .Include(o => o.User)
                .Where(o => o.Status == ApplicationStatus.Approved)
                .ToListAsync();
        }

        public async Task<List<OrganizerApplication>> GetRejectedOrganizerApplications()
        {
            return await _context.OrganizerApplications
    .Include(o => o.User)
    .Where(o => o.Status == ApplicationStatus.Rejected)
    .ToListAsync();
        }

        public async Task<OrganizerApplication?> ApproveApplication(int Applicationid)
        {
            return await _context.OrganizerApplications.FirstOrDefaultAsync(og => og.OrganizerApplicationId == Applicationid);
        }
    }
}
