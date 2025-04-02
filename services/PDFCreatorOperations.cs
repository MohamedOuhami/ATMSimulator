using System;
using System.Collections.Generic;
using System.IO;
using ATMSimulator.models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ATMSimulator.services
{
    public static class PDFCreatorOperations
    {

        public static void CreateWithdrawalReceipt(string accountNumber, decimal amount, decimal newBalance, string currency)
        {
            // Create document with small page size (like ATM receipt)
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Ouhami Banking Withdrawal Receipt";
            document.Info.Author = "Ouhami Banking";

            // ATM receipts are typically narrow (3.5" wide)
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromInch(3.5);
            page.Height = XUnit.FromInch(8); // Adjust height based on content

            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts
            XFont headerFont = new XFont("Courier New", 10, XFontStyleEx.Bold);
            XFont mainFont = new XFont("Courier New", 8, XFontStyleEx.Regular);
            XFont smallFont = new XFont("Courier New", 7, XFontStyleEx.Regular);

            XBrush blackBrush = XBrushes.Black; // Use XBrush for text

            XColor blackColor = XColors.Black; // Use XColor for drawing lines
            // Current timestamp for receipt
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss");

            // Margins and starting positions
            double margin = 10;
            double currentY = margin;
            double lineHeight = 14;
            double column1 = margin;
            double column2 = page.Width - margin - 100;

            // Draw ATM-style header
            gfx.DrawString("OUHAMI BANKING", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString("ATM WITHDRAWAL RECEIPT", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Draw separator line (using XColor.Black for the pen)
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Account information
            string maskedAccountNumber = accountNumber.Length > 8
                ? $"{accountNumber.Substring(0, 4)}****{accountNumber.Substring(accountNumber.Length - 4)}"
                : "****";  // If the account number is too short, hide it completely.

            gfx.DrawString($"ACCOUNT: {maskedAccountNumber}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString($"CURRENCY: {currency}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString($"DATE/TIME: {timestamp}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Another separator
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Withdrawal transaction details
            gfx.DrawString("WITHDRAWAL DETAILS:", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Amount withdrawn and new balance
            string amountText = $"{amount:0.00} {currency}";
            string newBalanceText = $"{newBalance:0.00} {currency}";

            gfx.DrawString($"AMOUNT WITHDRAWN: {amountText}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString($"NEW BALANCE: {newBalanceText}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Final separator
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Footer
            gfx.DrawString("THANK YOU FOR BANKING", headerFont, blackBrush,
                new XPoint((page.Width - margin * 2) / 2, currentY), XStringFormats.TopCenter);
            currentY += lineHeight;

            gfx.DrawString("WITH OUHAMI BANKING", headerFont, blackBrush,
                new XPoint((page.Width - margin * 2) / 2, currentY), XStringFormats.TopCenter);
            currentY += lineHeight * 1.5;

            // Save to desktop
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"ATM_Withdrawal_{now:yyyyMMddHHmmss}.pdf");

            document.Save(filePath);

            // Optionally, show message or perform further actions
            Console.WriteLine($"Receipt saved to: {filePath}");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });

        }

        public static void CreateAtmOperationsReceipt(List<Operation> last5Operations, string accountNumber, string currency)
        {
            // Create document with small page size (like ATM receipt)
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Ouhami Banking Receipt";
            document.Info.Author = "Ouhami Banking";

            // ATM receipts are typically narrow (3.5" wide)
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromInch(3.5);
            page.Height = XUnit.FromInch(8); // Adjust height based on content

            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts
            XFont headerFont = new XFont("Courier New", 10, XFontStyleEx.Bold);
            XFont mainFont = new XFont("Courier New", 8, XFontStyleEx.Regular);
            XFont smallFont = new XFont("Courier New", 7, XFontStyleEx.Regular);
            XBrush blackBrush = XBrushes.Black; // Use XBrush for text

            XColor blackColor = XColors.Black; // Use XColor for drawing lines

            // Current timestamp for receipt
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss");

            // Margins and starting positions
            double margin = 10;
            double currentY = margin;
            double lineHeight = 14;
            double column1 = margin;
            double column2 = page.Width - margin - 100;

            // Draw ATM-style header
            gfx.DrawString("OUHAMI BANKING", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString("ATM TRANSACTION RECEIPT", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Draw separator line (using XColor for the pen)
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Account information
            string maskedAccountNumber = accountNumber.Length > 3
                ? $"{accountNumber.Substring(0, 2)}****{accountNumber.Substring(accountNumber.Length - 4)}"
                : "****"; 

            gfx.DrawString($"ACCOUNT: {maskedAccountNumber}", mainFont, blackBrush, new XPoint(column1, currentY)); ;
            currentY += lineHeight;

            gfx.DrawString($"CURRENCY: {currency}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight;

            gfx.DrawString($"DATE/TIME: {timestamp}", mainFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Another separator
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Transaction header
            gfx.DrawString("LAST 5 TRANSACTIONS:", headerFont, blackBrush, new XPoint(column1, currentY));
            currentY += lineHeight * 1.5;

            // Transaction list
            foreach (var operation in last5Operations)
            {
                // Format amount with currency and 2 decimal places
                string amount = $"{operation.amount:0.00} {currency}";

                // Format date without seconds
                string opDate = operation.timestamp.ToString("yyyy-MM-dd HH:mm");

                // Draw transaction type and amount (right-aligned)
                gfx.DrawString($"{operation.type.ToUpper()}:", mainFont, blackBrush, new XPoint(column1, currentY));
                gfx.DrawString(amount, mainFont, blackBrush, new XPoint(column2, currentY), XStringFormats.TopRight);
                currentY += lineHeight;

                // Draw date below
                gfx.DrawString(opDate, smallFont, blackBrush, new XPoint(column1, currentY));
                currentY += lineHeight * 1.2;

                // Small separator between transactions
                if (operation != last5Operations[^1]) // If not last operation
                {
                    gfx.DrawLine(new XPen(blackColor, 0.2), column1, currentY, page.Width - margin, currentY);
                    currentY += lineHeight * 0.8;
                }
            }

            // Final separator
            currentY += lineHeight * 0.5;
            gfx.DrawLine(new XPen(blackColor, 0.5), column1, currentY, page.Width - margin, currentY);
            currentY += lineHeight;

            // Footer
            gfx.DrawString("THANK YOU FOR BANKING", headerFont, blackBrush,
                new XPoint((page.Width - margin * 2) / 2, currentY), XStringFormats.TopCenter);
            currentY += lineHeight;

            gfx.DrawString("WITH OUHAMI BANKING", headerFont, blackBrush,
                new XPoint((page.Width - margin * 2) / 2, currentY), XStringFormats.TopCenter);
            currentY += lineHeight * 1.5;

            gfx.DrawString($"RECEIPT ID: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                smallFont, blackBrush, new XPoint(column1, currentY));

            // Save to desktop
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"ATM_Receipt_{now:yyyyMMddHHmmss}.pdf");

            document.Save(filePath);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });

        }
    }
}
