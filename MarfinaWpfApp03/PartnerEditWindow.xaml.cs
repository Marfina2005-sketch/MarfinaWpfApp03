using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MarfinaWpfApp03
{
    public partial class PartnerEditWindow : Window
    {
        private readonly PartnerRepository _partnerRepository;
        private Partner _currentPartner;
        private readonly int? _partnerId;

        // Конструктор для добавления
        public PartnerEditWindow()
        {
            InitializeComponent();
            _partnerRepository = new PartnerRepository();
            TitleTextBlock.Text = "Добавление нового партнера";

            // Загружаем только типы, данные партнера не нужны
            LoadPartnerTypesAsync();
        }

        // Конструктор для редактирования 
        public PartnerEditWindow(int partnerId)
        {
            InitializeComponent();
            _partnerRepository = new PartnerRepository();
            _partnerId = partnerId;
            TitleTextBlock.Text = "Редактирование партнера";

            // Загружаем данные 
            LoadDataAsync();
        }

        // Загрузка типов партнеров
        private async void LoadPartnerTypesAsync()
        {
            try
            {
                var types = await _partnerRepository.GetAllPartnerTypesAsync();
                TypeComboBox.ItemsSource = types;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Последовательная загрузка всех данных
        private async void LoadDataAsync()
        {
            try
            {
                // Сначала загружаем типы
                await LoadPartnerTypesTaskAsync();

                // Потом загружаем данные партнера
                await LoadPartnerDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Task версия загрузки типов
        private async Task LoadPartnerTypesTaskAsync()
        {
            var types = await _partnerRepository.GetAllPartnerTypesAsync();
            TypeComboBox.ItemsSource = types;
        }

        // Task версия загрузки данных партнера
        private async Task LoadPartnerDataAsync()
        {
            _currentPartner = await _partnerRepository.GetPartnerByIdAsync(_partnerId.Value);

            if (_currentPartner != null)
            {
                TypeComboBox.SelectedValue = _currentPartner.TypeId;
                NameTextBox.Text = _currentPartner.Name;
                RatingTextBox.Text = _currentPartner.Rating.ToString();
                AddressTextBox.Text = _currentPartner.LegalAddress;
                DirectorTextBox.Text = _currentPartner.DirectorName;
                PhoneTextBox.Text = _currentPartner.Phone;
                EmailTextBox.Text = _currentPartner.Email;
            }
        }

        
        private async void LoadDataParallelAsync()
        {
            try
            {
                // Создаем отдельные репозитории для параллельных запросов
                var typesTask = LoadPartnerTypesWithNewRepositoryAsync();
                var partnerTask = LoadPartnerDataWithNewRepositoryAsync();

                await Task.WhenAll(typesTask, partnerTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadPartnerTypesWithNewRepositoryAsync()
        {
            using (var repository = new PartnerRepository())
            {
                var types = await repository.GetAllPartnerTypesAsync();

                // Обновляем UI в главном потоке
                Dispatcher.Invoke(() => {
                    TypeComboBox.ItemsSource = types;
                });
            }
        }

        private async Task LoadPartnerDataWithNewRepositoryAsync()
        {
            using (var repository = new PartnerRepository())
            {
                var partner = await repository.GetPartnerByIdAsync(_partnerId.Value);

                Dispatcher.Invoke(() => {
                    _currentPartner = partner;
                    if (partner != null)
                    {
                        TypeComboBox.SelectedValue = partner.TypeId;
                        NameTextBox.Text = partner.Name;
                        RatingTextBox.Text = partner.Rating.ToString();
                        AddressTextBox.Text = partner.LegalAddress;
                        DirectorTextBox.Text = partner.DirectorName;
                        PhoneTextBox.Text = partner.Phone;
                        EmailTextBox.Text = partner.Email;
                    }
                });
            }
        }

        // Сохранение
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (!ValidateInput())
                {
                    return;
                }

                // Создание или обновление объекта партнера
                var partner = _currentPartner ?? new Partner();

                partner.TypeId = (int)TypeComboBox.SelectedValue;
                partner.Name = NameTextBox.Text.Trim();
                partner.Rating = int.Parse(RatingTextBox.Text);
                partner.LegalAddress = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text.Trim();
                partner.DirectorName = string.IsNullOrWhiteSpace(DirectorTextBox.Text) ? null : DirectorTextBox.Text.Trim();
                partner.Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim();
                partner.Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text.Trim();

                if (_partnerId.HasValue)
                {
                    partner.Id = _partnerId.Value;
                    await _partnerRepository.UpdatePartnerAsync(partner);
                    MessageBox.Show("Данные партнера успешно обновлены!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _partnerRepository.AddPartnerAsync(partner);
                    MessageBox.Show("Партнер успешно добавлен!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Валидация ввода
        private bool ValidateInput()
        {
            if (TypeComboBox.SelectedValue == null)
            {
                ShowWarning("Выберите тип партнера");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowWarning("Введите наименование партнера");
                return false;
            }

            if (string.IsNullOrWhiteSpace(RatingTextBox.Text))
            {
                ShowWarning("Введите рейтинг партнера");
                return false;
            }

            if (!int.TryParse(RatingTextBox.Text, out int rating) || rating < 0)
            {
                ShowWarning("Рейтинг должен быть целым неотрицательным числом");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !IsValidEmail(EmailTextBox.Text))
            {
                ShowWarning("Введите корректный email адрес");
                return false;
            }

            return true;
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
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

        // Валидация ввода только чисел для рейтинга
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}