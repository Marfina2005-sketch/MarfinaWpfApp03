using System;

namespace MarfinaLibrary.Services
{
    public class DiscountService
    {
        /// <summary>
        /// Расчет скидки для партнера на основе общего объема продаж
        /// </summary>
        /// <param name="totalSalesAmount">Общая сумма продаж партнера</param>
        /// <returns>Размер скидки в процентах</returns>
        public int CalculateDiscount(decimal totalSalesAmount)
        {
            try
            {
                // Проверка на отрицательное значение
                if (totalSalesAmount < 0)
                {
                    throw new ArgumentException("Сумма продаж не может быть отрицательной");
                }

                // Расчет скидки по заданным условиям
                if (totalSalesAmount < 10000)
                {
                    return 0;
                }
                else if (totalSalesAmount >= 10000 && totalSalesAmount < 50000)
                {
                    return 5;
                }
                else if (totalSalesAmount >= 50000 && totalSalesAmount < 300000)
                {
                    return 10;
                }
                else // totalSalesAmount >= 300000
                {
                    return 15;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при расчете скидки: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение текстового описания скидки
        /// </summary>
        public string GetDiscountDescription(int discountPercent)
        {
            return discountPercent > 0 ? $"{discountPercent}%" : "нет";
        }
    }
}