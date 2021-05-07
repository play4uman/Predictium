using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Predictium.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Predictium.Output.Sheets
{
    public class SheetsOutput : ICanOutput, IDisposable
    {
        private static string[] scopes = { SheetsService.Scope.Spreadsheets };
        private static string applicationName = "Predictium";
        private static string spreadsheetId = "1nxSQVTxFMJB6LrCB4thQou6bNFmK_rEPEIYrKVwoWHA";
        private static string credentialsPath = "Output/Sheets/Creds/credentials.json";
        private static string dateColumn = "A";
        private static int headerRow = 1;
        private SheetsService sheetsService;

        public async Task InitializeAsync()
        {
            using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
            string credPath = "token.json";

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));

            sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }
        public async Task Output(IList<PredictionModel> predictionModels)
        {
            var groupingsByCryptoType = predictionModels
                .GroupBy(pm => pm.CryptoCurrencyCode);
            foreach (var predictionForCrypto in groupingsByCryptoType)
                await OutputCryptoCurrencyPredictions(predictionForCrypto, predictionForCrypto.Key);
            
        }

        private async Task OutputCryptoCurrencyPredictions(IEnumerable<PredictionModel> predictionModels,
            string cryptoCurrencyType)
        {
            int writeRow = await GetNextWriteRowNumberAsync(cryptoCurrencyType);
            var predictorColumnMapping = await GetPredictorColumnMappingAsync(cryptoCurrencyType);
            foreach (var predictionModel in predictionModels)
            {
                (string startColumn, string endColumn) = predictorColumnMapping[predictionModel.Author.Name];
                string range = $"{cryptoCurrencyType}!{startColumn}{writeRow}:{endColumn}{writeRow}";
                var valueRange = new ValueRange();
                var values = new List<object> { predictionModel.AveragePrice, predictionModel.ChangePercent };
                valueRange.Values = new List<IList<object>> { values };

                var writeRequest = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
                var writeResponse = await writeRequest.ExecuteAsync();
            }
        }

        private async Task<int> GetNextWriteRowNumberAsync(string cryptoCurrencyType)
        {
            string range = $"{cryptoCurrencyType}!{dateColumn}:{dateColumn}";
            var readReq = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var readResp = await readReq.ExecuteAsync();
            return readResp.Values[0].Count + 1;
        }

        private async Task<Dictionary<string, (string valueColumn, string percentColumn)>>
            GetPredictorColumnMappingAsync(string cryptoCurrencyType)
        {
            string range = $"{cryptoCurrencyType}!{headerRow}:{headerRow}";
            var readReq = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var readResp = await readReq.ExecuteAsync();
            var values = readResp.Values[0];

            var result = values
                .Where(val => val.ToString().ToUpper() != "DATE")
                .Select((val, index) => new { predictor = val.ToString(), index })
                .Where(pair => !string.IsNullOrEmpty(pair.predictor))
                .ToDictionary(
                    pair => pair.predictor,
                    pair => (ConvertIndexToColumnName(pair.index), ConvertIndexToColumnName(pair.index + 1))
                );
            return result;
        }

        private string ConvertIndexToColumnName(int index)
        {
            return ((char)(char.GetNumericValue('A') + index)).ToString();
        }

        public void Dispose()
        {
            sheetsService.Dispose();
        }
    }
}
