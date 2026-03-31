using Microsoft.EntityFrameworkCore;
using MarfinaLibrary.Database;
using MarfinaLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarfinaLibrary.Repositories
{
    public class PartnerRepository : IDisposable
    {
        private readonly MarfinaDbContext _context;
        private bool _disposed = false;

        public PartnerRepository()
        {
            _context = new MarfinaDbContext();
        }

        // Получение всех партнеров с их типами
        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            try
            {
                return await _context.PartnersMarfina
                    .Include(p => p.PartnerType)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка партнеров: {ex.Message}");
            }
        }

        // Получение партнера по ID
        public async Task<Partner> GetPartnerByIdAsync(int id)
        {
            try
            {
                return await _context.PartnersMarfina
                    .Include(p => p.PartnerType)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении партнера: {ex.Message}");
            }
        }

        // Добавление нового партнера
        public async Task AddPartnerAsync(Partner partner)
        {
            try
            {
                ValidatePartner(partner);
                await _context.PartnersMarfina.AddAsync(partner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении партнера: {ex.Message}");
            }
        }

        // Обновление данных партнера
        public async Task UpdatePartnerAsync(Partner partner)
        {
            try
            {
                ValidatePartner(partner);
                _context.PartnersMarfina.Update(partner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении партнера: {ex.Message}");
            }
        }

        // Удаление партнера
        public async Task DeletePartnerAsync(int id)
        {
            try
            {
                var partner = await GetPartnerByIdAsync(id);
                if (partner != null)
                {
                    _context.PartnersMarfina.Remove(partner);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении партнера: {ex.Message}");
            }
        }

        // Получение всех типов партнеров
        public async Task<List<PartnerType>> GetAllPartnerTypesAsync()
        {
            try
            {
                return await _context.PartnerTypesMarfina
                    .OrderBy(pt => pt.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении типов партнеров: {ex.Message}");
            }
        }

        // Валидация данных партнера
        private void ValidatePartner(Partner partner)
        {
            if (string.IsNullOrWhiteSpace(partner.Name))
                throw new ArgumentException("Наименование партнера обязательно для заполнения");

            if (partner.Rating < 0)
                throw new ArgumentException("Рейтинг не может быть отрицательным");

            if (!string.IsNullOrWhiteSpace(partner.Email) && !IsValidEmail(partner.Email))
                throw new ArgumentException("Некорректный формат email");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Реализация IDisposable
        public void Dispose()
        {
            if (!_disposed)
            {
                _context?.Dispose();
                _disposed = true;
            }
        }
    }
}