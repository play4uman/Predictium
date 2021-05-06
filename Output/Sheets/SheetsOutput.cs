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
        public async Task Output(IEnumerable<PredictionModel> predictionModels)
        {
            string range = "MySheet!A1:E1";
            var valueRange = new ValueRange();
            // Data for another Student...
            var oblist = new List<object>() { "Harry", "80", "77", "62", "98" };
            valueRange.Values = new List<IList<object>> { oblist };
            // Append the above record...
            var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = await appendRequest.ExecuteAsync();
        }

        public void Dispose()
        {
            sheetsService.Dispose();
        }
    }
}
