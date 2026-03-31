using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using MarfinaLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace MarfinaWpfApp03.ViewModels
{
    public class PartnerViewModel : INotifyPropertyChanged
    {
        private readonly PartnerRepository _partnerRepository;
        private readonly SalesRepository _salesRepository;
        private readonly DiscountService _discountService;
        private readonly SalesService _salesService;

        private ObservableCollection<PartnerDisplayModel> _partners;
        private PartnerDisplayModel _selectedPartner;
        private ObservableCollection<SalesDisplayModel> _salesHistory;
        private SalesDisplayModel _selectedSale;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public PartnerViewModel()
        {
            _partnerRepository = new PartnerRepository();
            _salesRepository = new SalesRepository();
            _discountService = new DiscountService();
            _salesService = new SalesService();

            Partners = new ObservableCollection<PartnerDisplayModel>();
            SalesHistory = new ObservableCollection<SalesDisplayModel>();

            // Загрузка данных при инициализации
            _ = LoadPartnersAsync();
        }

        public ObservableCollection<PartnerDisplayModel> Partners
        {
            get => _partners;
            set
            {
                _partners = value;
                OnPropertyChanged();
            }
        }

        public PartnerDisplayModel SelectedPartner
        {
            get => _selectedPartner;
            set
            {
                _selectedPartner = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _ = LoadSalesHistoryAsync(value.Id);
                }
            }
        }

        public SalesDisplayModel SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SalesDisplayModel> SalesHistory
        {
            get => _salesHistory;
            set
            {
                _salesHistory = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        // Загрузка списка партнеров
        private async Task LoadPartnersAsync()
        {
            try
            {
                IsLoading = true;
                Partners.Clear();

                var partners = await _partnerRepository.GetAllPartnersAsync();

                foreach (var partner in partners)
                {
                    var totalSales = await _salesRepository.GetTotalSalesByPartnerAsync(partner.Id);
                    var discountPercent = _discountService.CalculateDiscount(totalSales);

                    Partners.Add(new PartnerDisplayModel
                    {
                        Id = partner.Id,
                        Name = partner.Name,
                        TypeName = partner.PartnerType?.Name ?? "Не указан",
                        Rating = partner.Rating,
                        Phone = partner.Phone,
                        Email = partner.Email,
                        DirectorName = partner.DirectorName,
                        LegalAddress = partner.LegalAddress,
                        DiscountPercent = discountPercent,
                        DiscountDescription = _discountService.GetDiscountDescription(discountPercent)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Загрузка истории продаж для выбранного партнера
        private async Task LoadSalesHistoryAsync(int partnerId)
        {
            try
            {
                IsLoading = true;
                SalesHistory.Clear();

                var sales = await _salesService.GetSalesHistoryForPartnerAsync(partnerId);

                foreach (var sale in sales)
                {
                    SalesHistory.Add(sale);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Обновление списка партнеров
        public async Task RefreshPartnersAsync()
        {
            await LoadPartnersAsync();
        }

        // Добавление нового партнера
        public async Task<bool> AddPartnerAsync(Partner partner)
        {
            try
            {
                await _partnerRepository.AddPartnerAsync(partner);
                await RefreshPartnersAsync();

                MessageBox.Show("Партнер успешно добавлен!",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Обновление партнера
        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                await _partnerRepository.UpdatePartnerAsync(partner);
                await RefreshPartnersAsync();

                MessageBox.Show("Данные партнера успешно обновлены!",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Удаление партнера
        public async Task<bool> DeletePartnerAsync(int id)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить партнера?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _partnerRepository.DeletePartnerAsync(id);
                    await RefreshPartnersAsync();

                    MessageBox.Show("Партнер успешно удален!",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public async Task RefreshSalesHistoryAsync(int partnerId)
        {
            try
            {
                var salesService = new SalesService();
                var sales = await salesService.GetSalesHistoryForPartnerAsync(partnerId);
                SalesHistory = new ObservableCollection<SalesDisplayModel>(sales);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке истории продаж: {ex.Message}");
            }
        }

        public async Task DeleteSaleAsync(int saleId, int partnerId)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту продажу?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var salesRepository = new SalesRepository();
                    await salesRepository.DeleteSaleAsync(saleId);
                    await RefreshSalesHistoryAsync(partnerId);

                    MessageBox.Show("Продажа успешно удалена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении продажи: {ex.Message}");
            }
        }
    }

    // Модель для отображения партнера в UI
    public class PartnerDisplayModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public int Rating { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DirectorName { get; set; }
        public string LegalAddress { get; set; }
        public int DiscountPercent { get; set; }
        public string DiscountDescription { get; set; }

        public string DisplayName => $"{Name} (скидка: {DiscountDescription})";
    }
    
}