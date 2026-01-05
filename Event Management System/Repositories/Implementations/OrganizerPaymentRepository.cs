using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Event_Management_System.Repositories.Implementations
{
    public class OrganizerPaymentRepository : IOrganizerPaymentRepository
    {
        public readonly ApplicationDbContext _context;
    public OrganizerPaymentRepository(ApplicationDbContext context) {
        _context = context;
        }

        public async Task AddAsync(OrganizerPayment payment)
        {
            _context.organizerPayments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfPaymentEntryAlreadyExist(int applicationId)
        {
            return await _context.organizerPayments
                .AnyAsync(p => p.OrganizerApplicationId == applicationId);
        }

        public async Task<OrganizerPayment?> GetByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.organizerPayments
                .Include(p => p.OrganizerApplication)
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);
        }

        public async  Task UpdateAsync(OrganizerPayment payment)
        {
            _context.organizerPayments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}

