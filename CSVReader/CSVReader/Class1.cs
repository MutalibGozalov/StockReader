using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using StockReader.Application.Abstracts;
using StockReader.Domain.Entities;
using System.Globalization;

namespace CSVReader
{
    public class CSVReader : IDataReader
    {
        public string ReaderName { get; set; } = ".CSV Reader";
        public async IAsyncEnumerable<TradeData> ReadInputFiles(string filePath, int refreshRate = 5)
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

        public Task<List<TradeData>> ReadInputFilesAsync(string filePath, int refreshRate = 5)
        {
            throw new NotImplementedException();
        }
    }
}
