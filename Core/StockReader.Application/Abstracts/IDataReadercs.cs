using StockReader.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockReader.Application.Abstracts
{
    public interface IDataReader
    {
        public string ReaderName { get; set; }
        public IAsyncEnumerable<TradeData> ReadInputFiles(string filePath, int refreshRate = 5);
        public Task<List<TradeData>> ReadInputFilesAsync(string filePath, int refreshRate = 5);

    }
}
