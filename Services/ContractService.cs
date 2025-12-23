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
                    
                    var booking = context.Booking
                        .Include("Object")
                        .Include("User")
                        .Include("Object.Owner")
                        .Include("Object.Status")
                        .Include("Object.TypeOfSubject")
                        .FirstOrDefault(b => b.Id == bookingId);

                    if (booking == null) return (false, "Запись бронирования не найдена.", null);

                    
                    if (agentEntity == null) return (false, "Ошибка: Данные агента не переданы.", null);
                    if (booking.Object == null) return (false, "Ошибка: Объект недвижимости не привязан к брони.", null);

                   
                    var dto = new ContractPdfDto
                    {
                        
                        SigningDate = DateTime.Now,

                       
                        ObjectAddress = booking.Object.Address,
                        ObjectType = booking.Object.TypeOfSubject?.Name ?? "Не указан",
                        Area = booking.Object.Area,
                        RoomCount = booking.Object.RoomCount,
                        OwnerName = booking.Object.Owner?.FullName ?? "Не указан",
                        OwnerPassport = booking.Object.Owner?.Passport ?? "Не указан",

                       
                        ClientName = booking.User?.FullName ?? "Не указан",
                        ClientPassport = booking.User?.Passport ?? "Не указан",
                        ClientPhone = booking.User?.PhoneNumber ?? "Не указан",
                        ClientEmail = booking.User?.Email ?? "Не указан",

                     
                        AgentName = agentEntity.FullName ?? "Не указан",
                        AgentPhone = agentEntity.PhoneNumber ?? "Не указан",
                        AgentEmail = agentEntity.Email ?? "Не указан",

                      
                        Amount = (decimal)booking.Object.Price
                    };

                    
                    var contract = new Contract
                    {
                        SigningDate = dto.SigningDate,
                        Amount = (int)dto.Amount,
                        UserId = booking.UserId,
                        ObjectId = booking.ObjectId
                    };
                    context.Contract.Add(contract);

                 
                    var obj = context.Object.Find(booking.ObjectId);
                    if (obj != null) obj.StatusId = 3;

                  
                    context.Booking.Remove(booking);

                  
                    context.SaveChanges();

                    
                    dto.ContractId = contract.Id;

                    
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