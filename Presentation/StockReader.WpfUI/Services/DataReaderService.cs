using StockReader.Application.Abstracts;
using StockReader.WpfUI.ViewModels;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace StockReader.WpfUI.Services
{
    public class DataReaderService
    {
        private readonly IDataReader _txtreader;

        //internal static string pluginDirectory = "C:\\Users\\Mutalib\\Desktop\\Projects\\StockReader_xalqbank\\CSVReader\\CSVReader\\bin\\Debug\\net8.0";
        internal static string pluginDirectory = "C:\\Users\\Mutalib\\Desktop\\Projects\\Plugins";
        internal static Dictionary<string, IDataReader> Plugins = new Dictionary<string, IDataReader>();

        public DataReaderService(IDataReader txtreader)
        {
            _txtreader = txtreader;
        }

        public async void UpdateGrid(ObservableCollection<GridViewModel> itemSource, string inputDir)
        {
            await foreach (var item in _txtreader.ReadInputFiles(inputDir))
            {
                itemSource.Add(new GridViewModel
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

        public static void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        internal static void LoadPlugins()
        {
            foreach (var dll in Directory.GetFiles(pluginDirectory))
            {
                AssemblyLoadContext loadContext = new AssemblyLoadContext(dll);
                Assembly assembly = loadContext.LoadFromAssemblyPath(dll);
                var types = assembly.GetTypes();
                var asgt = types.Where(t => !t.IsAbstract && !t.IsInterface && t.IsAssignableFrom(typeof(IDataReader)));
                var type = asgt.FirstOrDefault();
                if (type is not null)
                {
                }
                    IDataReader reader = Activator.CreateInstance(assembly.GetType("CSVReader.CSVReader")) as IDataReader;
                    if (!Plugins.ContainsKey(Path.GetFileNameWithoutExtension(dll)))
                    {
                        Plugins.Add(Path.GetFileNameWithoutExtension(dll), reader);
                    }
            }
        }
    }
}
