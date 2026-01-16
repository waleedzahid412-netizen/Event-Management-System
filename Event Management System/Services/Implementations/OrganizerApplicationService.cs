using Event_Management_System.DTOs;
using Event_Management_System.Models;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Implementations;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using Event_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_System.Services.Implementations
{
    public class OrganizerApplicationService : IOrganizerApplicationService
    {
        public readonly IOrganizerApplicationRepository _organizerApplicationRepository;
        public readonly IUserRepository _userrepository;
        public readonly IEmailService _emailService;
        public OrganizerApplicationService(IOrganizerApplicationRepository organizerApplicationRepository,IUserRepository userRepository, IEmailService emailService)
        {
            _organizerApplicationRepository = organizerApplicationRepository;
            _userrepository = userRepository;
            _emailService = emailService;
        }

        public async Task<OrganizerApplicationCreateDTO> GetOrganizerApplicationByIdAsync(int applicationid)
        {
            var og = await _organizerApplicationRepository.GetOrganizerApplicationByIdAsync(applicationid);
            return new OrganizerApplicationCreateDTO
            {
                OrganizationName = og.OrganizationName,
                ContactEmail = og.ContactEmail,
                ContactPhone = og.ContactPhone,
                ExperienceDescription = og.ExperienceDescription,
            };
        }

        public async Task<int> SubmitOrganizerApplication(OrganizerApplicationCreateDTO dto, int Userid)
        {
            bool haspending = await _organizerApplicationRepository.CheckIfOrganizerApplicationExistAsync(Userid);
            if (haspending)
            {
                throw new Exception("You have already submitted an application. Please wait for review.");

            }
            var application = new OrganizerApplication
            {
                UserId = Userid,
                OrganizationName = dto.OrganizationName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                ExperienceDescription = dto.ExperienceDescription,
                WebsiteUrl = dto.WebsiteUrl,
            };
            await _organizerApplicationRepository.AddApplicationAsync(application);
            await _organizerApplicationRepository.SaveChangesAsync();
            return application.OrganizerApplicationId;

        }
        public Task<bool> checkIfApplicationExistButPaymentPending(int userid)
        {
            return _organizerApplicationRepository.checkIfApplicationExistButPaymentPending(userid);

        }
        public async Task<int> GetApplicationIdOfUser(int userid)
        {
            return await _organizerApplicationRepository.GetApplicationIdOfUser(userid);
        }
        public async Task<List<OrganizerApplicationListVM>> GetApplicationsByStatus(ApplicationStatus status)
        {
            List<OrganizerApplication> applications;
            if (status == ApplicationStatus.Pending)
            {
                applications = await _organizerApplicationRepository.GetPendingOrganizerApplications();
            }
            else if (status == ApplicationStatus.Approved)
            {
                applications = await _organizerApplicationRepository.GetApprovedOrganizerApplications();

            }
            else {
                applications = await _organizerApplicationRepository.GetRejectedOrganizerApplications();
            }


            return applications.Select(app => new OrganizerApplicationListVM
            {
                Id = app.OrganizerApplicationId,
                ApplicantName = app.User.FullName,
                OrganizationName = app.OrganizationName,
                Status = app.Status,
                AppliedOn = app.AppliedOn
            }).ToList();
        }
        public async Task<OrganizerApplicationsDetailVM?> GetApplicationDetailsAsync(int id)
        {
            var app = await _organizerApplicationRepository.GetOrganizerApplicationByIdAsync(id);
            if (app == null) return null;

            return new OrganizerApplicationsDetailVM
            {
                OrganizerApplicationId = app.OrganizerApplicationId,
                Status = app.Status,
                IsPaymentCompleted = app.IsPaymentCompleted,

                UserId = app.UserId,
                ApplicantName = app.User.FullName,
                ApplicantEmail = app.User.Email,

                OrganizationName = app.OrganizationName,
                ContactEmail = app.ContactEmail,
                ContactPhone = app.ContactPhone,
                ExperienceDescription = app.ExperienceDescription,
                WebsiteUrl = app.WebsiteUrl,

                ReviewedByAdminName = app.ReviewedByAdmin?.FullName,
                AdminComments = app.AdminComments,
                ReviewedOn = app.ReviewedOn,

                AppliedOn = app.AppliedOn
            };
        }
        public async Task<bool> ApproveApplicationAsync(int applicationId,int userid,string email, int adminId)
        {
            try
            {
                var application = await _organizerApplicationRepository.GetOrganizerApplicationByIdAsync(applicationId);

                if (application == null) return false;

                application.Status = ApplicationStatus.Approved;
                application.ReviewedByAdminId = adminId;
                application.ReviewedOn = DateTime.UtcNow;

                var userrole = await _userrepository.GetUserRoleFromId(userid);
                if (userrole == null) return false;
                userrole.RoleId = 2;
                await _emailService.OrganizerApprovalEmail(applicationId, email);
                await _userrepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // You can log the error here using ILogger or any logging framework
                // Example: _logger.LogError(ex, "Error approving application {ApplicationId}", applicationId);
                return false; // Return false if something goes wrong
            }
        }
        public async Task<bool> RejectApplicationAsync(int applicationId,string useremail, int adminId, string comments)
        {
            try
            {
                var application = await _organizerApplicationRepository.GetOrganizerApplicationByIdAsync(applicationId);

                if (application == null) return false;

                application.Status = ApplicationStatus.Rejected;
                application.ReviewedByAdminId = adminId;
                application.ReviewedOn = DateTime.UtcNow;
                application.AdminComments = comments;
                await _emailService.OrganizerRejectionEmail(application, useremail);
                await _userrepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }


    }
}
