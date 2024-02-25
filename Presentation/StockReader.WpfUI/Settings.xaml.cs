using Microsoft.Win32;
using StockReader.WpfUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockReader.WpfUI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        public Settings(AppSettings appSettings)
        {
            InitializeComponent();
            inputDirectory.Text = appSettings.InputDirectory;
        }

        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = "Foo",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                Multiselect = true
            };

            string folderName = "";
            if (dialog.ShowDialog() == true)
            {
                folderName = dialog.FolderName;
                inputDirectory.Text = folderName;
                DataReaderService.AddOrUpdateAppSetting("AppSettings:InputDirectory", folderName);
            }
        }
    }
}
