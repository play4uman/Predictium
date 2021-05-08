using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Output.Sheets.Creds
{
    public class SheetView
    {
        public SheetView(SheetsOutput parent, string title)
        {
            Parent = parent;
            Title = title;
        }

        public SheetsOutput Parent { get; set; }
        public string Title { get; set; }
        public string DateColumn { get; set; } = "A";
        public int HeaderRow { get; set; } = 1;
        private string ParentSheetId { get => Parent.SpreadsheetId; }
        private SheetsService SheetsService { get => Parent.SheetsService; }

        private Dictionary<string, (string valueColumn, string percentColumn)> 
            predictorColumnMapping;
        private Dictionary<DateTime, int> dateRowMapping;

        public async Task InitializeAsync()
        {
            predictorColumnMapping = await GetPredictorColumnMappingAsync(Title);
            dateRowMapping = await GetDateRowMapping();
        }
        public async Task OutputCryptoCurrencyPredictions(IEnumerable<PredictionModel> predictionModels)
        {
            int writeRow = -1;
            DateTime previousPredictionDate = default;

            foreach (var predictionModel in predictionModels.OrderBy(p => p.Date))
            {
                DateTime dateComponent = predictionModel.Date.Date;
                (string startColumn, string endColumn) = predictorColumnMapping[predictionModel.Author.Name];

                if (previousPredictionDate != dateComponent)
                {
                    writeRow = dateRowMapping[dateComponent];
                }
                string range = $"{Title}!{startColumn}{writeRow}:{endColumn}{writeRow}";
                var valueRange = new ValueRange();
                var values = new List<object> { predictionModel.AveragePrice, predictionModel.ChangePercent };
                valueRange.Values = new List<IList<object>> { values };

                var writeRequest = SheetsService.Spreadsheets.Values.Append(valueRange, ParentSheetId, range);
                writeRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var writeResponse = await writeRequest.ExecuteAsync();
            }
        }


        private async Task<Dictionary<string, (string valueColumn, string percentColumn)>>
            GetPredictorColumnMappingAsync(string sheetName)
        {
            string range = $"{sheetName}!{HeaderRow}:{HeaderRow}";
            var readReq = SheetsService.Spreadsheets.Values.Get(ParentSheetId, range);
            var readResp = await readReq.ExecuteAsync();
            var values = readResp.Values[0];

            var result = values
                .Where(val => val.ToString().ToUpper() != "DATE")
                .Select((val, index) => new { predictor = val.ToString(), index })
                .Where(pair => !string.IsNullOrEmpty(pair.predictor))
                .ToDictionary(
                    pair => pair.predictor,
                    pair => (ConvertIndexToColumnName(pair.index + 1), ConvertIndexToColumnName(pair.index + 2))
                );
            return result;
        }

        private string ConvertIndexToColumnName(int index)
        {
            return ((char)((int)'A' + index)).ToString();
        }

        private async Task<Dictionary<DateTime, int>> GetDateRowMapping()
        {
            string range = $"{Title}!{DateColumn}:{DateColumn}";
            var readReq = SheetsService.Spreadsheets.Values.Get(ParentSheetId, range);
            var readResp = await readReq.ExecuteAsync();
            var result = readResp.Values
                .Skip(2)
                .Select((dateStr, index) => 
                    (Date: DateTime.ParseExact(dateStr[0].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture), 
                        Index: index))
                .ToDictionary(
                    pair => pair.Date,
                    pair => pair.Index + 3
                );
            return result;
        }
    }
}
