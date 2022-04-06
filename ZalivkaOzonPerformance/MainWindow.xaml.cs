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
using System.Net.Http;
using System.Net;
using System.Net.Http.Formatting;


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
        static string Host = "https://performance.ozon.ru";
        private TokenClass Token;
        private List<CampaignIdFromOzon> CampaignIds;
        List<List<string>> PhrasesList;
        List<List<string>> BidsList;
        private List<CampaignForCreate> Campaigns;
        public MainWindow()
        {
            InitializeComponent();

            //For Design
            ClientID = "2459664-1649015479995@advertising.performance.ozon.ru";
            ClientSecret = "KI_vU3aIEv3Awxi0AO33kJoLcnjFh0YzSJeeyobpD6rxAw9Qmkk8OdA2mSXSW5bgxgmPmVnf6QIBo3wMrA";
            FullSpreadsheetRef = "https://docs.google.com/spreadsheets/d/1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA/edit#gid=0";
            SpreadsheetId = "1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA";
            PhrasesPageName = "Фразы";
            BidsPageName = "Ставки";

        }
        private List<CampaignForCreate> GetData(string SpreadsheetId, string PhrasesPageName, string BidsPageName)
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

            List<CampaignForCreate> campaignList;
            List<Product> productList;
            List<Phrase> phrasesList;

            IList<IList<Object>> bidsPageData = response.Values;
            string campaignPrefix;
            string campaignName = "Ozon Adversiting Mediator";
            int ic;
            ic = 0;//для ограничения на 250 товаров.
            int r;
            int i;
            List<object> BidsItemsList;
            //спарить фразы и ставки
            if (phrasesPageData != null && phrasesPageData.Count > 0)
            {
                campaignList = new List<CampaignForCreate>();
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
                                    if (item.ToString() != "" && i < BidsItemsList.Count - 1 && BidsItemsList[i].ToString() != "")
                                    {
                                        if (i > 2) //Фразы и ставки за фразы начинаются с третьего столбца в табличке
                                        {
                                            phrasesList.Add(new Phrase(BidsItemsList[i].ToString(), item.ToString()));
                                            //MessageBox.Show(String.Format("{0}, {1}, {2}", row[0].ToString(), item.ToString(), BidsItemsList[i].ToString()));
                                        }
                                    }
                                }
                            }
                            productList.Add(new Product(row[0].ToString(), "25", phrasesList));
                            if (productList.Count > 250 * ic)
                            {
                                string cn;
                                if (ic == 0)
                                    cn = campaignName;
                                else
                                    cn = campaignName + (ic + 1);
                                campaignList.Add(new CampaignForCreate(cn, "500", "DAILY_BUDGET", "PLACEMENT_SEARCH_AND_CATEGORY", ""));

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
        private async Task<TokenClass> GetToken(string clientID, string clientSecret)
        {
            TokenClass token;
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(Host + "/api/client/token");//https://performance.ozon.ru/api/client/token
            request.Method = HttpMethod.Post;
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Content = new StringContent("{\"client_id\":\"" + clientID + "\",\"client_secret\":\"" + clientSecret + "\",\"grant_type\":\"client_credentials\"}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.PostAsync(request.RequestUri, request.Content).Result;//почему-то при вызове метода программа зависает. Видимо, нет ответа? Возможно, ошибка в request.Content

            token = await response.Content.ReadAsAsync<TokenClass>();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return token;
            }

            MessageBox.Show(String.Format("Токен будет работать {0} секунд = {1} минут.", token.expires_in.ToString(), token.expires_in / 60));
            return token;

        }
        private async Task<List<CampaignIdFromOzon>> CreateCampaign(List<CampaignForCreate> campaigns, TokenClass token)
        {
            CampaignIdFromOzon cid;
            List<CampaignIdFromOzon> campaignIdList = new List<CampaignIdFromOzon>();
            string campaignId;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = new HttpResponseMessage();
            request.RequestUri = new Uri("https://performance.ozon.ru:443/api/client/campaign/cpm/product");//https://performance.ozon.ru:443/api/client/campaign/cpm/product
            request.Method = HttpMethod.Post;

            foreach (CampaignForCreate campaign in campaigns)
            {
                request.Content = new StringContent("{\"title\":\"" + campaign.title + "\",\"dailyBudget\":\"" + campaign.dailyBudget + "\",\"placement\":\"" + campaign.placement + "\",\"expenseStrategy\":\"" + campaign.expenseStrategy + "\"}",
                    Encoding.UTF8, "application/json");

                response = httpClient.PostAsync(request.RequestUri, request.Content).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    MessageBox.Show(response.StatusCode.ToString());

                    break;
                }

                cid = await response.Content.ReadAsAsync<CampaignIdFromOzon>();//не стринг, а список кампаний: https://docs.ozon.ru/api/performance/#operation/ListCampaigns
                if (cid.campaignId != "")
                    campaignIdList.Add(cid);
                MessageBox.Show(cid.campaignId);
            }
            return campaignIdList;
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //Получить кампании
            Campaigns = GetData(SpreadsheetId, PhrasesPageName, BidsPageName);
            //Получить товары
            Token = GetToken(ClientID, ClientSecret).GetAwaiter().GetResult();
            //создать РК, получить их ID
            CampaignIds = CreateCampaign(Campaigns, Token).GetAwaiter().GetResult();

            if (Campaigns.Count != CampaignIds.Count)
            {
                MessageBox.Show("Количество РК и ID разное!");
                return;
            }
            else
            {
                for (int i = 0; i <= Campaigns.Count - 1; i++)
                {
                    Campaigns[i].campaignId = CampaignIds[i].campaignId;
                    AddProductsInCampaign(Campaigns[i], Token);
                }
            }


            //залить в созданную РК товары
            //разделить получение токена и создане РК
            //создать таймер токена
            //запросить список РК - позже
            //найти РК по названию - позже
            //если нет рк, то предложить создать - позже
            //если есть, то предложить удаление всех товаров из РК? - позже
        }
        private async void AddProductsInCampaign(CampaignForCreate campaignForCreate, TokenClass token)
        {
            CampaignIdFromOzon cid;
            List<CampaignIdFromOzon> campaignIdList = new List<CampaignIdFromOzon>();
            string campaignId;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);

            ProductsInCampaignOzon productsInCampaignOzon = new ProductsInCampaignOzon(campaignForCreate);

            response = await httpClient.PostAsJsonAsync<ProductsInCampaignOzon>("https://performance.ozon.ru:443/api/client/campaign/" + productsInCampaignOzon.campaignId + "/products", productsInCampaignOzon);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                MessageBox.Show("Товары заведены в Кампанию с ID:" + productsInCampaignOzon.campaignId);
            }
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("https://docs.google.com/document/d/1RgoFvJecjq2P-ErS_DFdICO0ohjl2RrOD2DH4SawY2g/edit");
        }
    }
}
