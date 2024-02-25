using StockReader.Application.Abstracts;
using StockReader.WpfUI.Services;
using StockReader.WpfUI.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;


namespace StockReader.WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<GridViewModel> gridViewModel = new ObservableCollection<GridViewModel>();
        private readonly DataReaderService _dataReaderService;
        private readonly AppSettings _appSettings;
        public MainWindow(DataReaderService dataReaderService, AppSettings appSettings)
        {
            InitializeComponent();
            txtGrid.ItemsSource = gridViewModel;
            _dataReaderService = dataReaderService;
            _appSettings = appSettings;
        }



        private async void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataReaderService.UpdateGrid(gridViewModel, _appSettings.InputDirectory);

            DataReaderService.LoadPlugins();

            foreach (var reader in DataReaderService.Plugins.Keys)
            {
                await foreach (var item in DataReaderService.Plugins[reader].ReadInputFiles(_appSettings.InputDirectory))
                {
                    gridViewModel.Add(new GridViewModel
                    {
                        TradeDate = item.TradeDate,
                        Open = item.Open,
                        High = item.High,
                        Low = item.Low,
                        Close = item.Close,
                        Volume = item.Volume
                    });
                }
            }
        }

       
    }
}