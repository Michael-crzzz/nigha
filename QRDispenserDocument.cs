using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace Dashboard.Document
{
    public class QRDispenserDocument : IDocument
    {
        private readonly string _dispenserId;
        private readonly string _location;
        private readonly string _unit;
        private readonly string _qrBase64; // ⬅ this will come from your DB (e.g., dispenser.QRCodeImageBase64)

        public QRDispenserDocument(string dispenserId, string location, string unit, string qrBase64)
        {
            _dispenserId = dispenserId;
            _location = location;
            _unit = unit;
            _qrBase64 = qrBase64;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            // Convert base64 QR image to byte[] so we can embed it
            byte[] qrImageBytes = Convert.FromBase64String(_qrBase64);

            container.Page(page =>
            {
                page.Margin(0);
                page.Background("#00457C");

                page.Content().Padding(30).Column(column =>
                {
                    // Title
                    column.Item().AlignCenter().Text("QR CODE for DISPENSER")
                                 .FontSize(22).Bold().FontColor(Colors.White);

                    column.Item().PaddingBottom(10).AlignCenter().Text($"ID: {_dispenserId}")
                                 .FontSize(14).FontColor(Colors.White);

                    // QR Section
                    column.Item().AlignCenter().Border(1).BorderColor(Colors.White).Padding(20)
                        .Width(150).Height(150)
                        .Background(Colors.White)
                        .Column(qrColumn =>
                        {
                            qrColumn.Item().AlignCenter().Image(qrImageBytes, ImageScaling.FitArea); // ← uses the exact image from DB
                        });

                    // Info Section
                    column.Item().PaddingTop(20).Column(info =>
                    {
                        info.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Location: ").SemiBold().FontColor(Colors.White);
                            text.Span(_location).FontColor(Colors.White);
                        });

                        info.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Unit: ").SemiBold().FontColor(Colors.White);
                            text.Span(_unit).FontColor(Colors.White);
                        });
                    });

                    // Footer text
                    column.Item().PaddingTop(40).AlignCenter().Text("QR CODE")
                                 .FontSize(36).Bold().FontColor(Colors.White);
                });
            });
        }
    }
}
