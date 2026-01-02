using Event_Management_System.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EventManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventImage> eventimage { get;set; }
        public DbSet<PaymentReciept> PaymentReceipts { get; set; }
        public DbSet<OrganizerApplication> OrganizerApplications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================================
            // USER → USERROLES (1 to many)
            // ================================
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // ROLE → USERROLES (1 to many)
            // ================================
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // EVENTCATEGORY → EVENTS (1 to many)
            // ================================
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================
            // USER (Organizer) → EVENTS (1 to many)
            // ================================
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================
            // EVENT → REGISTRATIONS (1 to many)
            // ================================
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // USER → REGISTRATIONS (1 to many)
            // ================================
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // REGISTRATION → PAYMENT (1 to 1)
            // ================================
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Registration)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // EVENT → NOTIFICATIONS (1 to many)
            // ================================
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Event)
                .WithMany(e => e.Notifications)
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // USER → NOTIFICATIONS (1 to many)
            // ================================
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Event>()
                 .HasMany(e => e.EventImages)
                 .WithOne(img => img.Event)
                 .HasForeignKey(img => img.EventId)
                 .OnDelete(DeleteBehavior.Cascade);

            // ORGANIZERAPPLICATION → USER (Applicant) (many to 1)
            modelBuilder.Entity<OrganizerApplication>()
                .HasOne(app => app.User)
                .WithMany(u => u.OrganizerApplications)
                .HasForeignKey(app => app.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // ORGANIZERAPPLICATION → USER (Reviewed By Admin) (many to 1, optional)
            // ================================
            modelBuilder.Entity<OrganizerApplication>()
                .HasOne(app => app.ReviewedByAdmin)
                .WithMany() // no collection needed
                .HasForeignKey(app => app.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
