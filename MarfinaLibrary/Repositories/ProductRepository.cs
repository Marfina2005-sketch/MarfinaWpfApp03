using Microsoft.EntityFrameworkCore;
using MarfinaLibrary.Database;
using MarfinaLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarfinaLibrary.Repositories
{
    public class ProductRepository
    {
        private readonly MarfinaDbContext _context;

        public ProductRepository()
        {
            _context = new MarfinaDbContext();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.ProductsMarfina
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка продуктов: {ex.Message}");
            }
        }
    }
}