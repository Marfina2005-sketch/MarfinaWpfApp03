using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarfinaLibrary.Services
{
    public class SalesService
    {
        private readonly SalesRepository _salesRepository;

        public SalesService()
        {
            _salesRepository = new SalesRepository();
        }

        // Получение истории продаж для отображения
        public async Task<List<SalesDisplayModel>> GetSalesHistoryForPartnerAsync(int partnerId)
        {
            try
            {
                var sales = await _salesRepository.GetSalesHistoryByPartnerAsync(partnerId);
                var displayList = new List<SalesDisplayModel>();

                foreach (var sale in sales)
                {
                    displayList.Add(new SalesDisplayModel
                    {
                        Id = sale.Id,
                        ProductName = sale.Product?.Name ?? "Неизвестный товар",
                        Quantity = sale.Quantity,
                        SaleDate = sale.SaleDate,
                        TotalAmount = sale.TotalAmount
                    });
                }

                return displayList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении истории продаж: {ex.Message}");
            }
        }
    }

    // Модель для отображения в UI
    public class SalesDisplayModel
    {

        public int Id { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }
    }
}