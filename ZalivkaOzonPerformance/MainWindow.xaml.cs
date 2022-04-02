using System;
using System.IO;
using System.Threading;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;


//API key AIzaSyBcZzPmjDXPxWTKKKIesPIneb4iaWaiPmA
//id таблицы spreadsheetId  1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA
namespace ZalivkaOzonPerformance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        private string ClientID;
        private string ClientSecret;
        private string FullSpreadsheetRef;
        private string SpreadsheetId;
        private string PhrasesPageName;
        private string BidsPageName;
        List<List<string>> PhrasesList;
        List<List<string>> BidsList;
        private List<Campaign> Campaigns;
        public MainWindow()
        {
            InitializeComponent();

            //For Design
            ClientID = "utUYsOG4Mp2JxkCPqhrC57UIgPIq5tWx0LXeDDY9FncuEdpLxJIfD0-LQhJL4OlVJHsntvy00wifv_i32Q";
            ClientSecret = "utUYsOG4Mp2JxkCPqhrC57UIgPIq5tWx0LXeDDY9FncuEdpLxJIfD0-LQhJL4OlVJHsntvy00wifv_i32Q";
            FullSpreadsheetRef = "https://docs.google.com/spreadsheets/d/1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA/edit#gid=0";
            SpreadsheetId = "1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA";
            PhrasesPageName = "testPhrases";
            BidsPageName = "testBids";

        }

        private List<Campaign> GetData(string SpreadsheetId, string PhrasesPageName, string BidsPageName)
        {
            UserCredential credential;

            List<List<string>> rowsList;
            List<string> itemsList;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //MessageBox.Show("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String range;
            String spreadsheetId = SpreadsheetId;
            ValueRange response;
            SpreadsheetsResource.ValuesResource.GetRequest request;

            //извлечь фразы
            range = PhrasesPageName + "!A3:CY10000";
            //"test!A3:CY10000";
            request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // https://docs.google.com/spreadsheets/d/1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA/edit
            response = request.Execute();
            IList<IList<Object>> phrasesPageData = response.Values;

            //извлечь ставки
            range = BidsPageName + "!A3:CY10000";
            //"test!A3:CY10000"; =
            request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // https://docs.google.com/spreadsheets/d/1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA/edit
            response = request.Execute();

            List<Campaign> campaignList;
            List<Product> productList;
            List<Phrase> phrasesList;

            IList<IList<Object>> bidsPageData = response.Values;
            string campaignPrefix;
            string campaignName;
            int ic;
            ic = 1;//для ограничения на 250 товаров.
            int r;
            int i;
            List<object> BidsItemsList;
            //спарить фразы и ставки
            if (phrasesPageData != null && phrasesPageData.Count > 0)
            {
                campaignList = new List<Campaign>();
                productList = new List<Product>();
                foreach (var row in phrasesPageData)
                {
                    if (row != null && row.Count > 0)
                    {
                        r = phrasesPageData.IndexOf(row);
                        if (row.First().ToString() == bidsPageData[r].First().ToString())
                        {
                            phrasesList = new List<Phrase>();

                            foreach (var item in row)
                            {
                                if (item != null)
                                {
                                    i = row.IndexOf(item);
                                    BidsItemsList = new List<object>(bidsPageData[r]);
                                    if (item.ToString() != "" && i<BidsItemsList.Count-1 && BidsItemsList[i].ToString() != "")
                                    {
                                        if (i > 2) //Фразы и ставки за фразы начинаются с третьего столбца в табличке
                                        {
                                            phrasesList.Add(new Phrase(BidsItemsList[i].ToString(), item.ToString()));
                                            MessageBox.Show(String.Format("{0}, {1}, {2}", row[0].ToString(), item.ToString(), BidsItemsList[i].ToString()));
                                        }
                                    }
                                }
                            }
                            productList.Add(new Product(row[0].ToString(), "25", phrasesList));
                            if (productList.Count > 250 * ic)
                            {
                                if (ic == 1)
                                    campaignName = "тест";
                                else
                                    campaignName = "тест" + ic;
                                campaignList.Add(new Campaign(campaignName, "500", "DAILY_BUDGET", "PLACEMENT_SEARCH_AND_CATEGORY", ""));

                                ic++;
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
                MessageBox.Show("No data found PhrasesPageName.");
            }

            return campaignList;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Campaigns = GetData(SpreadsheetId, PhrasesPageName, BidsPageName);

            //попробуем вывести в табличку?
            //запросить список РК - позже
            //найти РК по названию - позже
            //если нет рк, то предложить создать - позже
            //если есть, то предложить удаление всех товаров из РК? - позже
            //создать РК
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("https://docs.google.com/document/d/1RgoFvJecjq2P-ErS_DFdICO0ohjl2RrOD2DH4SawY2g/edit");
        }
    }
}
