using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using StockReader.Application.Abstracts;
using StockReader.Domain.Entities;
using System.Globalization;

namespace StockReader.Infrastructure.Concretes
{
    public class TxtDataReader : IDataReader
    {
        public string ReaderName { get; set; } = ".TXT Reader";
        public async IAsyncEnumerable<TradeData> ReadInputFiles(string filePath, int refreshRate = 5)
        {
            DirectoryInfo directory = new DirectoryInfo(filePath);
            FileInfo[] files = directory.GetFiles(searchPattern: "*.txt");
            foreach (FileInfo file in files)
            {
                var content = await File.ReadAllLinesAsync(file.FullName);
                foreach (var item in content)
                {
                    if (!DateOnly.TryParse(item.Split(';')[0], out _))
                        continue;
                    await Task.Delay(500);
                    yield return new TradeData
                    {
                        TradeDate = DateOnly.Parse(item.Split(';')[0]),
                        Open = decimal.Parse(item.Split(';')[1]),
                        High = decimal.Parse(item.Split(';')[2]),
                        Low = decimal.Parse(item.Split(';')[3]),
                        Close = decimal.Parse(item.Split(';')[4]),
                        Volume = long.Parse(item.Split(';')[5])
                    };
                }
            }
        }

        public async IAsyncEnumerable<TradeData> ReadInputFiles2(string filePath, int refreshRate = 5)
        {
            DirectoryInfo directory = new DirectoryInfo(filePath);
            FileInfo[] files = directory.GetFiles(searchPattern: "*.csv");
            foreach (FileInfo file in files)
            {


                List<string[]> data = new List<string[]>();

                // Read the CSV file using TextFieldParser
                using (TextFieldParser parser = new TextFieldParser(file.FullName))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Skip the header row
                    parser.ReadLine();

                    // Read the rest of the file
                    while (!parser.EndOfData)
                    {
                        // Read one line
                        string[] fields = parser.ReadFields();


                        await Task.Delay(500);
                        yield return new TradeData
                        {
                            TradeDate = DateOnly.Parse(fields[0]),
                            Open = decimal.Parse(fields[1]),
                            High = decimal.Parse(fields[2]),
                            Low = decimal.Parse(fields[3]),
                            Close = decimal.Parse(fields[4]),
                            Volume = long.Parse(fields[5])
                        };
                    }
                }
            }
        }

        public async Task<List<TradeData>> ReadInputFilesAsync(string filePath, int refreshRate = 5)
        {
            List<TradeData> data = new List<TradeData>();
            DirectoryInfo directory = new DirectoryInfo(filePath);
            FileInfo[] files = directory.GetFiles(searchPattern: "*.txt");
            foreach (FileInfo file in files)
            {
                var content = await File.ReadAllLinesAsync(file.FullName);

                    foreach (var item in content)
                    {
                        if (!DateOnly.TryParse(item.Split(';')[0], out _))
                            continue;
                        await Task.Delay(300);
                        data.Add(new TradeData
                        {
                            TradeDate = DateOnly.Parse(item.Split(';')[0]),
                            Open = decimal.Parse(item.Split(';')[1]),
                            High = decimal.Parse(item.Split(';')[2]),
                            Low = decimal.Parse(item.Split(';')[3]),
                            Close = decimal.Parse(item.Split(';')[4]),
                            Volume = long.Parse(item.Split(';')[5])
                        });
                    }

            }
            return data;
        }
    }
}
