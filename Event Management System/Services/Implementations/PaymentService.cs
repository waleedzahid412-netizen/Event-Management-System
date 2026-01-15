using Event_Management_System.DTOs;
using Event_Management_System.Models.Entities;
using Event_Management_System.Repositories.Interfaces;
using Event_Management_System.Services.Interfaces;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Event_Management_System.ViewModels;

namespace Event_Management_System.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
   
        public readonly IRegistrationRepository _registrationRepository;

        public readonly IPaymentRecieptRepository _paymentrepo;


        public PaymentService(IEventRepository rep, IRegistrationRepository rrep,
            IUserRepository userRepository
            , IPaymentRecieptRepository paymentrepo, IOrganizerApplicationRepository repo
            , IEventReviewRepository eventreviewrepo, IEventCategoryRepository eventcatrepo)
        {

            _registrationRepository = rrep;

            _paymentrepo = paymentrepo;
        }
        public async Task<(PaymentReciept receipt, byte[] pdfBytes)> CreateCustomerPaymentReceiptAsync(
                   int userId,
                   EventDetailsVM ev,
                   List<Registration> registrations
)
        {

            var receiptData = new PaymentReceiptDTO
            {
                UserEmail = await _registrationRepository.useremail(userId),
                EventTitle = ev.Title,
                NumberOfTickets = registrations.Count,
                TicketPrice = ev.TicketPrice,
                TicketNumbers = registrations.Select(r => r.TicketNumber).ToList()
            };

            var pdfBytes = GenerateCustomerRegistationReceiptPdf(receiptData);


            var receiptEntity = new PaymentReciept
            {
                UserId = userId,
                EventId = ev.EventId,
                NumberOfTickets = registrations.Count,
                TicketPrice = ev.TicketPrice,
                TotalAmount = registrations.Count * ev.TicketPrice,
                ReceiptPdf = pdfBytes
            };

            await _paymentrepo.AddPaymentReciept(receiptEntity);
            await _paymentrepo.SaveChangesAsync();

            return (receiptEntity, pdfBytes);
        }
        public byte[] GenerateCustomerRegistationReceiptPdf(PaymentReceiptDTO receipt)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var receiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    // ===== HEADER =====
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("EVENT MANAGEMENT SYSTEM")
                                .FontSize(18)
                                .Bold();

                            col.Item().Text("Official Payment Receipt")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(200).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Receipt No: {receiptNumber}")
                                .FontSize(10);

                            col.Item().Text($"Date: {DateTime.UtcNow:dd MMM yyyy}")
                                .FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(15);

                        // ===== CUSTOMER DETAILS =====
                        col.Item().Text("Billing Information")
                            .Bold()
                            .FontSize(12);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                        {
                            c.Spacing(5);
                            c.Item().Text($"Email Address: {receipt.UserEmail}");
                            c.Item().Text($"Event Name: {receipt.EventTitle}");
                            c.Item().Text($"Payment Date: {DateTime.UtcNow:dd MMM yyyy, HH:mm} UTC");
                        });

                        // ===== PAYMENT SUMMARY =====
                        col.Item().Text("Payment Summary")
                            .Bold()
                            .FontSize(12);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            table.Cell().Text("Tickets Purchased");
                            table.Cell().AlignRight().Text(receipt.NumberOfTickets.ToString());

                            table.Cell().Text("Price per Ticket");
                            table.Cell().AlignRight().Text($"${receipt.TicketPrice:F2}");
                            table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1);

                            table.Cell().Text("Total Amount Paid").Bold();
                            table.Cell().AlignRight().Text($"${receipt.TotalAmount:F2}").Bold();
                        });

                        // ===== TICKET NUMBERS =====
                        col.Item().Text("Issued Ticket Numbers")
                            .Bold()
                            .FontSize(12);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(ticketCol =>
                        {
                            ticketCol.Spacing(4);
                            foreach (var ticket in receipt.TicketNumbers)
                            {
                                ticketCol.Item().Text($"• {ticket}");
                            }
                        });
                    });

                    // ===== FOOTER =====
                    page.Footer().AlignCenter().Column(col =>
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().PaddingTop(5).Text("This receipt serves as official proof of payment.")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        col.Item().Text("© 2025 Event Management System. All rights reserved.")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });
                });
            }).GeneratePdf();
        }
        public byte[] GenerateOrganizerReceiptPdf(OrganizerPaymentReceiptDTO receipt)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var receiptNumber = $"ORG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    // ===== HEADER =====
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("EVENT MANAGEMENT SYSTEM")
                                .FontSize(18)
                                .Bold();

                            col.Item().Text("Organizer Application Payment Receipt")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(200).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Receipt No: {receiptNumber}")
                                .FontSize(10);

                            col.Item().Text($"Date: {DateTime.UtcNow:dd MMM yyyy}")
                                .FontSize(10);
                        });
                    });

                    // ===== CONTENT =====
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(15);

                        // ===== APPLICANT DETAILS =====
                        col.Item().Text("Applicant Information")
                            .Bold()
                            .FontSize(12);

                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Column(c =>
                            {
                                c.Spacing(5);
                                c.Item().Text($"Email Address: {receipt.UserEmail}");
                                c.Item().Text($"Organization Name: {receipt.OrganizationName}");
                                c.Item().Text($"Application ID: {receipt.ApplicationId}");
                                c.Item().Text($"Payment Date: {receipt.PaymentDate:dd MMM yyyy, HH:mm} UTC");
                            });

                        // ===== PAYMENT SUMMARY =====
                        col.Item().Text("Payment Summary")
                            .Bold()
                            .FontSize(12);

                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(120);
                                });

                                table.Cell().Text("Payment Purpose");
                                table.Cell().AlignRight().Text("Organizer Registration Fee");

                                table.Cell().Text("Amount Paid");
                                table.Cell().AlignRight().Text($"${receipt.AmountPaid:F2}");

                                table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1);

                                table.Cell().Text("Total Amount").Bold();
                                table.Cell().AlignRight().Text($"${receipt.AmountPaid:F2}").Bold();
                            });

                        // ===== STATUS =====
                        col.Item().Text("Payment Status")
                            .Bold()
                            .FontSize(12);

                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Green.Darken1)
                            .Padding(10)
                            .Text("✔ Payment Completed Successfully")
                            .FontColor(Colors.Green.Darken2)
                            .Bold();
                    });

                    // ===== FOOTER =====
                    page.Footer().AlignCenter().Column(col =>
                    {
                        col.Item().LineHorizontal(1);
                        col.Item().PaddingTop(5)
                            .Text("This receipt confirms successful payment for organizer application.")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        col.Item().Text("© 2025 Event Management System. All rights reserved.")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });
                });
            }).GeneratePdf();
        }
    }
}
