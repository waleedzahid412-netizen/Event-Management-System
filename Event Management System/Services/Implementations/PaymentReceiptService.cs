using Event_Management_System.DTOs;
using Event_Management_System.Models;
using Event_Management_System.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Event_Management_System.Services.Implementations
{
    public class PaymentReceiptService : IPaymentReceiptService
    {
        public byte[] GenerateReceiptPdf(PaymentReceiptDTO receipt)
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
    }
}
