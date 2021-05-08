using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Predictium.Models;
using Predictium.Output.Sheets.Creds;
using Predictium.Predictors;
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
        private static string credentialsPath = "Output/Sheets/Creds/credentials.json";
        public readonly string SpreadsheetId = "1nxSQVTxFMJB6LrCB4thQou6bNFmK_rEPEIYrKVwoWHA";
        public SheetsService SheetsService { get; private set; }
        public IEnumerable<SheetView> SheetViews { get; private set; }

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

            SheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });

            SheetViews = await InitializeSheetViewsAsync();
        }
        public async Task Output(IList<PredictionModel> predictionModels)
        {
            var sheetViewsAndCorrespondingModels = predictionModels
                .GroupBy(pm => pm.CryptoCurrencyCode)
                .Join(SheetViews, g => g.Key, sv => sv.Title, 
                    (g, sv) => new { Models = g, SheetView = sv});

            foreach (var sheetViewAndModels in sheetViewsAndCorrespondingModels)
                await sheetViewAndModels.SheetView.OutputCryptoCurrencyPredictions(sheetViewAndModels.Models);
        }


        private async Task<IEnumerable<string>> GetAllSheetTitlesAsync()
        {
            var sheetRequest = SheetsService.Spreadsheets.Get(SpreadsheetId);
            var sheetResponse = await sheetRequest.ExecuteAsync();
            return sheetResponse.Sheets.Select(s => s.Properties.Title);
        }

        private async Task<IEnumerable<SheetView>> InitializeSheetViewsAsync()
        {
            var sheetTitles = await GetAllSheetTitlesAsync();
            var sheetViews = sheetTitles
                .Select(sheetTitle => new SheetView(this, sheetTitle))
                .ToList();

            foreach (var sv in sheetViews)
                await sv.InitializeAsync();

            return sheetViews;
        }
        public void Dispose()
        {
            SheetsService.Dispose();
        }
    }
}
