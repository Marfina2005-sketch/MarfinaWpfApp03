using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using MarfinaLibrary.Services;
using MarfinaWpfApp03.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MarfinaWpfApp03
{
    public partial class MainWindow : Window
    {
        private PartnerViewModel _viewModel;
        private readonly SalesRepository _salesRepository = new SalesRepository();
        private readonly SalesService _salesService = new SalesService();

        public MainWindow()
        {
            InitializeComponent();

            // Создаем ViewModel
            _viewModel = new PartnerViewModel();

            // Устанавливаем DataContext
            DataContext = _viewModel;

            // подписка на изменение выбранного партнера
            _viewModel.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(PartnerViewModel.SelectedPartner) && _viewModel.SelectedPartner != null)
                {
                    await _viewModel.RefreshSalesHistoryAsync(_viewModel.SelectedPartner.Id);
                }
            };
        }


        // Обработчик добавления партнера
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editWindow = new PartnerEditWindow();
                editWindow.Owner = this;

                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем список после добавления
                    _ = _viewModel.RefreshPartnersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна добавления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик редактирования партнера
        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedPartner == null)
                {
                    MessageBox.Show("Выберите партнера для редактирования",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var editWindow = new PartnerEditWindow(_viewModel.SelectedPartner.Id);
                editWindow.Owner = this;

                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем список после редактирования
                    _ = _viewModel.RefreshPartnersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна редактирования: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик удаления партнера
        private async void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedPartner == null)
                {
                    MessageBox.Show("Выберите партнера для удаления",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _viewModel.DeletePartnerAsync(_viewModel.SelectedPartner.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void SalesGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.Row.Item is SalesDisplayModel row)
            {
                var sale = new SalesHistory
                {
                    
                    PartnerId = _viewModel.SelectedPartner.Id,
                    Quantity = row.Quantity,
                    SaleDate = row.SaleDate,
                    TotalAmount = row.TotalAmount
                };

                await _salesRepository.UpdateSaleAsync(sale);
            }
        }
        private string GenerateReport()
        {
            var partners = _viewModel.Partners; // Получаем список партнеров из ViewModel

            // Строим строку отчета
            var report = "Отчет о партнерах\n\n";
            foreach (var partner in partners)
            {
                report += $"Наименование: {partner.Name}\n";
                report += $"Тип: {partner.TypeName}\n";
                report += $"Рейтинг: {partner.Rating}\n";
                report += $"Телефон: {partner.Phone}\n";
                report += $"Email: {partner.Email}\n";
                report += $"Директор: {partner.DirectorName}\n";
                report += "----------------------\n";
            }

            return report; // Возвращаем строку отчета
        }
        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Генерация отчета
                var reportContent = GenerateReport();

                // Получаем путь к папке, где лежит проект 
                string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Формируем путь к файлу отчета
                string filePath = Path.Combine(projectDirectory, "report.txt");

                // Сохраняем отчет в текстовый файл
                System.IO.File.WriteAllText(filePath, reportContent);

                // Открываем файл с отчетом 
                System.Diagnostics.Process.Start("notepad.exe", filePath);

                // Подтверждение
                MessageBox.Show($"Отчет успешно сохранен по пути: {filePath}",
                                 "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}",
                                 "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void AddSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedPartner == null)
                {
                    MessageBox.Show("Выберите партнера", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saleWindow = new SaleEditWindow(_viewModel.SelectedPartner.Id);
                saleWindow.Owner = this;

                if (saleWindow.ShowDialog() == true)
                {
                    await _viewModel.RefreshSalesHistoryAsync(_viewModel.SelectedPartner.Id);
                    MessageBox.Show("Продажа успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedPartner == null)
                {
                    MessageBox.Show("Выберите партнера", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_viewModel.SelectedSale == null)
                {
                    MessageBox.Show("Выберите продажу для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saleWindow = new SaleEditWindow(_viewModel.SelectedPartner.Id, _viewModel.SelectedSale.Id);
                saleWindow.Owner = this;

                if (saleWindow.ShowDialog() == true)
                {
                    await _viewModel.RefreshSalesHistoryAsync(_viewModel.SelectedPartner.Id);
                    MessageBox.Show("Продажа успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedPartner == null)
                {
                    MessageBox.Show("Выберите партнера", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_viewModel.SelectedSale == null)
                {
                    MessageBox.Show("Выберите продажу для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _viewModel.DeleteSaleAsync(_viewModel.SelectedSale.Id, _viewModel.SelectedPartner.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении продажи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}