using StockReader.Application.Abstracts;
using StockReader.WpfUI.Services;
using StockReader.WpfUI.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


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

        DataGrid grid;
        public MainWindow(DataReaderService dataReaderService, AppSettings appSettings)
        {
            InitializeComponent();
            txtGrid.ItemsSource = gridViewModel;
            _dataReaderService = dataReaderService;
            _appSettings = appSettings;
        }


        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dataReaderService.UpdateGrid(gridViewModel, _appSettings.InputDirectory);

            DataReaderService.LoadPlugins();

            foreach (var reader in DataReaderService.Plugins.Keys)
            {
                ObservableCollection<GridViewModel> gridItemSource = new ObservableCollection<GridViewModel>();
                AddNewTab(gridItemSource, reader);
                await foreach (var item in DataReaderService.Plugins[reader].ReadInputFiles(_appSettings.InputDirectory))
                {
                    gridItemSource.Add(new GridViewModel
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
       
        private DataGrid AddNewTab(ObservableCollection<GridViewModel> gridItemSource, string tabHeader)
        {
            // Create new TabItem
            TabItem newTabItem = new TabItem();
            newTabItem.Header = tabHeader;

            // Create Grid
            Grid grid = new Grid();
            grid.Background = Brushes.LightGray;

            // Create Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);

            // Create DataGrid
            DataGrid dataGrid = new DataGrid();
            dataGrid.ItemsSource = gridItemSource;// Add your data source here
            dataGrid.Margin = new Thickness(0);
            dataGrid.AutoGenerateColumns = true; // Or set up columns manually

            // Add DataGrid to Border
            border.Child = dataGrid;

            // Add Border to Grid
            grid.Children.Add(border);

            // Set Grid as TabItem content
            newTabItem.Content = grid;

            // Add TabItem to TabControl
            tabControl.Items.Add(newTabItem);

            return dataGrid;
        }

        private void SetInputRefreshRate(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //refreshRateTxt.Content = $"Change input directory refresh rate: {e.NewValue}";
            DataReaderService.AddOrUpdateAppSetting("AppSettings:InputRefreshRate", e.NewValue);
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(_appSettings);
            settings.Show();
        }
    }
}