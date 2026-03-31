using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace MarfinaWpfApp03
{
    public partial class SaleEditWindow : Window, INotifyPropertyChanged
    {
        private readonly SalesRepository _salesRepository;
        private readonly ProductRepository _productRepository;
        private readonly int _partnerId;
        private readonly int? _saleId;
        private List<Product> _products;

        private int _productId;
        private int _quantity;
        private DateTime _saleDate;
        private decimal _totalAmount;
        private string _windowTitle;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); CalculateTotalAmount(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); CalculateTotalAmount(); }
        }

        public DateTime SaleDate
        {
            get => _saleDate;
            set { _saleDate = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; OnPropertyChanged(); }
        }

        public SaleEditWindow(int partnerId) : this(partnerId, null)
        {
        }

        public SaleEditWindow(int partnerId, int? saleId)
        {
            InitializeComponent();

            _salesRepository = new SalesRepository();
            _productRepository = new ProductRepository();
            _partnerId = partnerId;
            _saleId = saleId;

            DataContext = this;

            Loaded += async (s, e) => await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                _products = await _productRepository.GetAllProductsAsync();
                ProductComboBox.ItemsSource = _products;

                if (_saleId.HasValue)
                {
                    WindowTitle = "Редактирование продажи";
                    DeleteButton.Visibility = Visibility.Visible;

                    var sale = await _salesRepository.GetSaleByIdAsync(_saleId.Value);
                    if (sale != null)
                    {
                        ProductId = sale.ProductId;
                        Quantity = sale.Quantity;
                        SaleDate = sale.SaleDate;
                        TotalAmount = sale.TotalAmount;

                        if (sale.ProductId > 0)
                        {
                            ProductComboBox.SelectedValue = sale.ProductId;
                        }
                    }
                }
                else
                {
                    WindowTitle = "Добавление продажи";
                    SaleDate = DateTime.Today;
                    DeleteButton.Visibility = Visibility.Collapsed;
                }

                ProductComboBox.SelectionChanged += (s, e) => CalculateTotalAmount();
                QuantityTextBox.TextChanged += (s, e) => CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateTotalAmount()
        {
            if (ProductComboBox.SelectedValue is int productId && Quantity > 0)
            {
                var product = _products?.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    TotalAmount = product.MinPartnerPrice * Quantity;
                }
            }
            else
            {
                TotalAmount = 0;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите продукт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество (положительное число)",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!SaleDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату продажи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var product = _products.FirstOrDefault(p => p.Id == (int)ProductComboBox.SelectedValue);
                if (product == null)
                {
                    MessageBox.Show("Выбранный продукт не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var sale = new SalesHistory
                {
                    PartnerId = _partnerId,
                    ProductId = (int)ProductComboBox.SelectedValue,
                    Quantity = Quantity,
                    SaleDate = SaleDatePicker.SelectedDate.Value,
                    TotalAmount = product.MinPartnerPrice * Quantity
                };

                if (_saleId.HasValue)
                {
                    sale.Id = _saleId.Value;
                    await _salesRepository.UpdateSaleAsync(sale);
                    DialogResult = true;
                }
                else
                {
                    await _salesRepository.AddSaleAsync(sale);
                    DialogResult = true;
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_saleId.HasValue)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту продажу?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _salesRepository.DeleteSaleAsync(_saleId.Value);
                        DialogResult = true;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}