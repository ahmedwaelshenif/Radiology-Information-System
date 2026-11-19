
using Application.Interfaces.IServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;

namespace Infrastructure.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        public byte[] GeneratePdf(List<string> fields)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            if (fields == null || fields.Count == 0)
                throw new ArgumentException("PDF fields cannot be empty.");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Header()
                        .Text("Radiology Report")
                        .FontSize(24)
                        .Bold();

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            foreach (var field in fields)
                            {
                                column.Item()
                                    .PaddingVertical(5)
                                    .Text(field ?? string.Empty)
                                    .FontSize(12);
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9);
                });
            });

            return document.GeneratePdf();
        }
    }
}
