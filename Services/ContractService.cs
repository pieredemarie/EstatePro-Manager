using RealEstateAgency.DAL;
using RealEstateAgency.Models;
using RealEstateAgency.Services.Interfaces;
using System;
using System.Linq;

namespace RealEstateAgency.Services
{
    public class ContractService
    {
        private readonly IPdfExportService _pdfService;

        public ContractService(IPdfExportService pdfService)
        {
            _pdfService = pdfService;
        }

        public (bool Success, string Message, string PdfPath) CreateContractFromBooking(int bookingId, User agentEntity)
        {
            try
            {
                using (var context = new RealEstateDBEntity())
                {
                    // 1. Загружаем бронь со всеми связями
                    var booking = context.Booking
                        .Include("Object")
                        .Include("User")
                        .Include("Object.Owner")
                        .Include("Object.Status")
                        .Include("Object.TypeOfSubject")
                        .FirstOrDefault(b => b.Id == bookingId);

                    if (booking == null) return (false, "Запись бронирования не найдена.", null);

                    // ПРОВЕРКА: Если агент не передан, будет ошибка на FullName
                    if (agentEntity == null) return (false, "Ошибка: Данные агента не переданы.", null);
                    if (booking.Object == null) return (false, "Ошибка: Объект недвижимости не привязан к брони.", null);

                    // 2. СНАЧАЛА СОЗДАЕМ DTO (Пока данные в памяти и связаны)
                    // Используем оператор ?. на случай, если какие-то связи в базе пусты
                    var dto = new ContractPdfDto
                    {
                        // Данные контракта (Id присвоим позже или используем 0, так как в PDF он может быть не критичен до сохранения)
                        SigningDate = DateTime.Now,

                        // Объект
                        ObjectAddress = booking.Object.Address,
                        ObjectType = booking.Object.TypeOfSubject?.Name ?? "Не указан",
                        Area = booking.Object.Area,
                        RoomCount = booking.Object.RoomCount,
                        OwnerName = booking.Object.Owner?.FullName ?? "Не указан",
                        OwnerPassport = booking.Object.Owner?.Passport ?? "Не указан",

                        // Клиент
                        ClientName = booking.User?.FullName ?? "Не указан",
                        ClientPassport = booking.User?.Passport ?? "Не указан",
                        ClientPhone = booking.User?.PhoneNumber ?? "Не указан",
                        ClientEmail = booking.User?.Email ?? "Не указан",

                        // Агент
                        AgentName = agentEntity.FullName ?? "Не указан",
                        AgentPhone = agentEntity.PhoneNumber ?? "Не указан",
                        AgentEmail = agentEntity.Email ?? "Не указан",

                        // Деньги
                        Amount = (decimal)booking.Object.Price
                    };

                    // 3. ТЕПЕРЬ ДЕЛАЕМ ОПЕРАЦИИ В БД
                    // Создаем контракт
                    var contract = new Contract
                    {
                        SigningDate = dto.SigningDate,
                        Amount = (int)dto.Amount,
                        UserId = booking.UserId,
                        ObjectId = booking.ObjectId
                    };
                    context.Contract.Add(contract);

                    // Обновляем статус объекта
                    var obj = context.Object.Find(booking.ObjectId);
                    if (obj != null) obj.StatusId = 3;

                    // Удаляем бронь
                    context.Booking.Remove(booking);

                    // Сохраняем изменения
                    context.SaveChanges();

                    // Теперь, когда контракт сохранен, у него появился реальный ID из базы
                    dto.ContractId = contract.Id;

                    // 4. ГЕНЕРИРУЕМ PDF
                    string path = _pdfService.ExportContractPdf(dto);

                    return (true, null, path);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка: {ex.Message}", null);
            }
        }
    }
}