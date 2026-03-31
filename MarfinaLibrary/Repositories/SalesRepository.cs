using Microsoft.EntityFrameworkCore;
using MarfinaLibrary.Database;
using MarfinaLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MarfinaLibrary.Repositories
{
    public class SalesRepository
    {
        private readonly MarfinaDbContext _context;

        public SalesRepository()
        {
            _context = new MarfinaDbContext();
        }

        // Получение истории продаж для конкретного партнера
        public async Task<List<SalesHistory>> GetSalesHistoryByPartnerAsync(int partnerId)
        {
            try
            {
                return await _context.SalesHistoriesMarfina
                    .Include(s => s.Product)
                    .Where(s => s.PartnerId == partnerId)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении истории продаж: {ex.Message}");
            }
        }

        // Получение общей суммы продаж для партнера
        public async Task<decimal> GetTotalSalesByPartnerAsync(int partnerId)
        {
            try
            {
                return await _context.SalesHistoriesMarfina
                    .Where(s => s.PartnerId == partnerId)
                    .SumAsync(s => s.TotalAmount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при расчете общей суммы продаж: {ex.Message}");
            }
        }

        // Получение всех продаж с деталями
        public async Task<List<SalesHistory>> GetAllSalesWithDetailsAsync()
        {
            try
            {
                return await _context.SalesHistoriesMarfina
                    .Include(s => s.Partner)
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка продаж: {ex.Message}");
            }
        }
        public async Task UpdateSaleAsync(SalesHistory sale)
        {
            try
            {
                // Проверяем, отслеживается ли уже эта сущность
                var existingEntity = _context.SalesHistoriesMarfina.Local
                    .FirstOrDefault(e => e.Id == sale.Id);

                if (existingEntity != null)
                {
                    // Если отслеживается, отсоединяем её
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                // Присоединяем обновленную сущность в состоянии Modified
                _context.Entry(sale).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении продажи: {ex.Message}");
            }
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _context.SalesHistoriesMarfina.FindAsync(id);

            if (sale != null)
            {
                _context.SalesHistoriesMarfina.Remove(sale);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<SalesHistory> GetSaleByIdAsync(int id)
        {
            try
            {
                return await _context.SalesHistoriesMarfina
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении продажи: {ex.Message}");
            }
        }

        public async Task AddSaleAsync(SalesHistory sale)
        {
            try
            {
                await _context.SalesHistoriesMarfina.AddAsync(sale);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении продажи: {ex.Message}");
            }
        }
    }



}