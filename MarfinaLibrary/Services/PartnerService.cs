using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarfinaLibrary.Services
{
    public class PartnerService
    {
        private readonly PartnerRepository _partnerRepository;
        private readonly SalesRepository _salesRepository;
        private readonly DiscountService _discountService;

        public PartnerService()
        {
            _partnerRepository = new PartnerRepository();
            _salesRepository = new SalesRepository();
            _discountService = new DiscountService();
        }

        // Получение всех партнеров с рассчитанными скидками
        public async Task<List<Partner>> GetAllPartnersWithDiscountsAsync()
        {
            try
            {
                var partners = await _partnerRepository.GetAllPartnersAsync();

                // Для каждого партнера можно будет рассчитать скидку при отображении
                return partners;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в сервисе партнеров: {ex.Message}");
            }
        }

        // Получение партнера со скидкой
        public async Task<(Partner partner, int discountPercent)> GetPartnerWithDiscountAsync(int partnerId)
        {
            try
            {
                var partner = await _partnerRepository.GetPartnerByIdAsync(partnerId);
                if (partner == null)
                {
                    throw new Exception("Партнер не найден");
                }

                var totalSales = await _salesRepository.GetTotalSalesByPartnerAsync(partnerId);
                var discountPercent = _discountService.CalculateDiscount(totalSales);

                return (partner, discountPercent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении партнера со скидкой: {ex.Message}");
            }
        }
    }
}