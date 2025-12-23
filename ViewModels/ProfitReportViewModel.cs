using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32; 
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Kernel.Colors;
using RealEstateAgency.Services;
using RealEstateAgency.Services.Interfaces;

namespace RealEstateAgency.ViewModels
{
    public class ProfitReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly INavigationService _navigationService;
        private readonly IPdfExportService _pdfExportService; 

        private ObservableCollection<ProfitDataItem> _profitData;
        private List<int> _availableYears;
        private int _selectedYear;
        private decimal _totalProfit;
        private bool _isLoading;
        private bool _canExportToPdf;

        public ProfitReportViewModel()
        {
            _reportService = new ReportService();
            _navigationService = new NavigationService();
            _pdfExportService = new PdfExportService(); 

            ProfitData = new ObservableCollection<ProfitDataItem>();
            InitializeYears();
            InitializeCommands();
        }

       
        public ObservableCollection<ProfitDataItem> ProfitData
        {
            get => _profitData;
            set { _profitData = value; OnPropertyChanged(); }
        }

        public List<int> AvailableYears
        {
            get => _availableYears;
            set { _availableYears = value; OnPropertyChanged(); }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReportTitle));
            }
        }

        public decimal TotalProfit
        {
            get => _totalProfit;
            set { _totalProfit = value; OnPropertyChanged(); }
        }

        public string ReportTitle => $"Отчет по прибыли за {SelectedYear} год";

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGenerateReport));
            }
        }

        public bool CanExportToPdf
        {
            get => _canExportToPdf;
            set { _canExportToPdf = value; OnPropertyChanged(); }
        }

        public bool CanGenerateReport => !IsLoading;
      
        public ICommand GenerateReportCommand { get; private set; }
        public ICommand ExportToPdfCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        private void InitializeCommands()
        {
            GenerateReportCommand = new RelayCommand(GenerateReport, () => CanGenerateReport);
            ExportToPdfCommand = new RelayCommand(ExportToPdf, () => CanExportToPdf);
            RefreshCommand = new RelayCommand(GenerateReport, () => CanGenerateReport);
        }

        private void InitializeYears()
        {
            try
            {
                AvailableYears = _reportService.GetAvailableYears();
                SelectedYear = AvailableYears.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка лет: {ex.Message}");
                AvailableYears = new List<int> { DateTime.Now.Year };
                SelectedYear = DateTime.Now.Year;
            }
        }

        public void GenerateReport()
        {
            if (IsLoading) return;

            IsLoading = true;
            CanExportToPdf = false;
            ProfitData.Clear();
            TotalProfit = 0;

            Task.Run(() =>
            {
                try
                {
                    var data = _reportService.GetProfitReport(SelectedYear);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProfitData = new ObservableCollection<ProfitDataItem>(data);
                        TotalProfit = data.Sum(x => x.Profit);
                        CanExportToPdf = ProfitData.Any();
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        _navigationService.ShowMessage("Ошибка", ex.Message));
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsLoading = false;
                        CommandManager.InvalidateRequerySuggested();
                    });
                }
            });
        }

        private void ExportToPdf()
        {
            if (ProfitData == null || !ProfitData.Any()) return;

            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"Отчет_по_прибыли_{SelectedYear}.pdf",
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                string filePath = saveFileDialog.FileName;
                int year = SelectedYear;
                var data = ProfitData.ToList();
                decimal total = TotalProfit;

                Task.Run(() =>
                {
                    try
                    {
                        // Вся тяжелая работа теперь здесь
                        _pdfExportService.ExportProfitReport(filePath, year, data, total);

                        Application.Current.Dispatcher.Invoke(() =>
                            _navigationService.ShowMessage("Успех", $"Файл сохранен:\n{filePath}"));
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            _navigationService.ShowMessage("Ошибка", $"Ошибка экспорта: {ex.Message}"));
                    }
                    finally
                    {
                        Application.Current.Dispatcher.Invoke(() => IsLoading = false);
                    }
                });
            }
        }
    }


    public class ProfitDataItem : ViewModelBase
    {
        private string _objectTypeName;
        private int _dealCount;
        private decimal _profit;
        private double _percentage;
        private Color _chartColor;

        
        public string ObjectTypeName
        {
            get => _objectTypeName;
            set
            {
                _objectTypeName = value;
                OnPropertyChanged();
            }
        }

        
        public int DealCount
        {
            get => _dealCount;
            set
            {
                _dealCount = value;
                OnPropertyChanged();
            }
        }

     
        public decimal Profit
        {
            get => _profit;
            set
            {
                _profit = value;
                OnPropertyChanged();
            }
        }

       
        public double Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                OnPropertyChanged();
            }
        }

        
        public Color ChartColor
        {
            get => _chartColor;
            set
            {
                _chartColor = value;
                OnPropertyChanged();
            }
        }
    }

}
