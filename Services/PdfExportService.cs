using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using RealEstateAgency.Models;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.ViewModels;
using System;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RealEstateAgency.Services
{
    public class PdfExportService : IPdfExportService
    {
        public void ExportProfitReport(string fileName, int year, List<ProfitDataItem> data, decimal totalProfit)
        {
            using (var writer = new PdfWriter(fileName))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                if (!File.Exists(fontPath))
                    fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf");

                PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                document.SetFont(font);

                
                document.Add(new Paragraph("Отчет по прибыли")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Отчетный период: {year} год")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetMarginBottom(20));

                
                document.Add(new Paragraph($"Общая прибыль агентства: {totalProfit:N0} ₽")
                    .SetFontSize(15)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetMarginBottom(30));

               
                Table table = new Table(new float[] { 3, 2, 2, 2 }).UseAllAvailableWidth();
                Color headerBg = ColorConstants.LIGHT_GRAY;

                table.AddHeaderCell(new Cell().Add(new Paragraph("Тип объекта")).SetBackgroundColor(headerBg));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Сделки")).SetBackgroundColor(headerBg));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Выручка")).SetBackgroundColor(headerBg));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Доля")).SetBackgroundColor(headerBg));

                foreach (var item in data)
                {
                    table.AddCell(new Cell().Add(new Paragraph(item.ObjectTypeName)));
                    table.AddCell(new Cell().Add(new Paragraph(item.DealCount.ToString())));
                    table.AddCell(new Cell().Add(new Paragraph($"{item.Profit:N0} ₽")));
                    table.AddCell(new Cell().Add(new Paragraph($"{item.Percentage:F1}%")));
                }

                document.Add(table);

               
                document.Add(new Paragraph("\nСтатистика:")
                    .SetFontSize(14)
                    .SetMarginTop(20));

                document.Add(new Paragraph($"Всего типов объектов: {data.Count}"));
                document.Add(new Paragraph($"Всего закрытых сделок: {data.Sum(x => x.DealCount)}"));

                var leader = data.OrderByDescending(x => x.Profit).FirstOrDefault();
                if (leader != null)
                {
                    document.Add(new Paragraph($"Лидер продаж: {leader.ObjectTypeName} ({leader.Percentage:F1}%)"));
                }

                
                document.Add(new Paragraph($"\n\nДокумент сформирован автоматически: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.RIGHT));
            }
        }

        public string ExportContractPdf(ContractPdfDto model)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Договоры");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, $"Договор_№{model.ContractId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                PdfFont boldFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                document.SetFont(font);

                
                document.Add(new Paragraph("ДОГОВОР АРЕНДЫ НЕДВИЖИМОСТИ")
                    .SetFont(boldFont).SetFontSize(16).SetTextAlignment(TextAlignment.CENTER).SetMarginBottom(20));

                document.Add(new Paragraph($"Договор №: {model.ContractId}\nДата подписания: {model.SigningDate:dd.MM.yyyy}")
                    .SetTextAlignment(TextAlignment.RIGHT).SetMarginBottom(20));

                document.Add(new iText.Layout.Element.LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()));

                document.Add(new Paragraph("1. ОБЪЕКТ НЕДВИЖИМОСТИ").SetFont(boldFont).SetMarginTop(10));
                document.Add(new Paragraph($"Адрес: {model.ObjectAddress}\nТип: {model.ObjectType}\nПлощадь: {model.Area} м², Комнат: {model.RoomCount}\nСобственник: {model.OwnerName} (Паспорт: {model.OwnerPassport})"));

                
                document.Add(new Paragraph("2. АРЕНДАТОР (КЛИЕНТ)").SetFont(boldFont).SetMarginTop(10));
                document.Add(new Paragraph($"ФИО: {model.ClientName}\nПаспорт: {model.ClientPassport}\nТелефон: {model.ClientPhone}\nEmail: {model.ClientEmail}"));

              
                document.Add(new Paragraph("3. АГЕНТ").SetFont(boldFont).SetMarginTop(10));
                document.Add(new Paragraph($"ФИО: {model.AgentName}\nТелефон: {model.AgentPhone}\nEmail: {model.AgentEmail}"));

               
                document.Add(new Paragraph("4. УСЛОВИЯ И ОПЛАТА").SetFont(boldFont).SetMarginTop(10));
                document.Add(new Paragraph($"Стоимость объекта: {model.Amount:N0} ₽"));
                document.Add(new Paragraph($"Комиссия агентства (3%): {model.Commission:N0} ₽"));

                
                document.Add(new Paragraph($"ИТОГО К ОПЛАТЕ: {model.Total:N0} ₽")
                    .SetFontSize(14)
                    .SetFont(boldFont)
                    .SetMarginBottom(30));

              
                document.Add(new Paragraph("\n\n\n"));

                Table signTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();

                signTable.AddCell(new Cell()
                    .Add(new Paragraph("Заказчик: "))
                    .Add(new Paragraph("\n_________________ / "))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                signTable.AddCell(new Cell()
                    .Add(new Paragraph("Исполнитель: "))
                    .Add(new Paragraph("\n_________________ / "))
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                document.Add(signTable);

                document.Close();
            }
            return filePath;
        }
    }
}