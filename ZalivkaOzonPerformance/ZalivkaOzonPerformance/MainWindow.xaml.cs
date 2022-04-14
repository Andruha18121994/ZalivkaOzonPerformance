using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Net.Http;
using System.Net;
using System.Net.Http.Formatting;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Unicode;


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

        private string ClientID = string.Empty;
        private string ClientSecret = string.Empty;
        private string SpreadsheetId = string.Empty;
        private string PhrasesPageName = string.Empty;
        private string BidsPageName = string.Empty;
        static string Host = "https://performance.ozon.ru";
        private string CampaignName = string.Empty;
        private TokenClass Token = new TokenClass();
        private List<CampaignIdFromOzon> CampaignIds = new List<CampaignIdFromOzon>();
        private List<CampaignForCreate> Campaigns = new List<CampaignForCreate>();

        public MainWindow()
        {
            InitializeComponent();

            //For Design
            ClientIdTextBox.Text = "2459664-1649015479995@advertising.performance.ozon.ru";
            ClientSecretTextBox.Text = "KI_vU3aIEv3Awxi0AO33kJoLcnjFh0YzSJeeyobpD6rxAw9Qmkk8OdA2mSXSW5bgxgmPmVnf6QIBo3wMrA";
            SpreadsheetIdTextBox.Text = "1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA";
            PhrasesPageNameTextBox.Text = "Фразы";
            BidsPageNameTextBox.Text = "Ставки";
            CampaignNameTextBox.Text = "Test Ozon Advertising Mediator";
            MessageBox.Show("Введены данные по умолчанию - для тестирования");
        }


        private List<CampaignForCreate> GetData(string SpreadsheetId, string PhrasesPageName, string BidsPageName, string CampaignName)
        {
            UserCredential credential;

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
            response = request.Execute();//error
            IList<IList<Object>> phrasesPageData = response.Values;

            //извлечь ставки
            range = BidsPageName + "!A3:CY10000";
            //"test!A3:CY10000"; =
            request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // https://docs.google.com/spreadsheets/d/1ClieG7on-ov-YslrKFrAtpvMFKV1AYh1fx4P5x5ZxWA/edit
            response = request.Execute();

            List<CampaignForCreate> campaignList;
            List<Phrase> phrasesList;

            IList<IList<Object>> bidsPageData = response.Values;
            int ic;
            ic = 0;//для ограничения на 250 товаров.
            int r;
            int i;
            List<object> BidsItemsList;
            //спарить фразы и ставки
            if (phrasesPageData != null && phrasesPageData.Count > 0)
            {
                campaignList = new List<CampaignForCreate>();
                if (phrasesPageData.Count > 0)
                {
                    string cn;
                    cn = CampaignName;
                    //создать РК 
                    CampaignForCreate campaignForCreate = new CampaignForCreate(cn, "500000000", "DAILY_BUDGET", "PLACEMENT_SEARCH_AND_CATEGORY");

                    foreach (var row in phrasesPageData)
                    {
                        //если в кампании 250 товаров, то добавить кампанию в список и создать новую РК.
                        if (campaignForCreate.products.Count == 250)
                        {
                            cn = CampaignName + (ic + 1);
                            campaignList.Add(campaignForCreate);
                            campaignForCreate = new CampaignForCreate(cn, "500000000", "DAILY_BUDGET", "PLACEMENT_SEARCH_AND_CATEGORY");
                            ic++;
                        }

                        if (row != null && row.Count > 0)
                        {
                            r = phrasesPageData.IndexOf(row);
                            if (row.First() != null && row.First().ToString() != "" && row.First().ToString() == bidsPageData[r].First().ToString())
                            {
                                phrasesList = new List<Phrase>();
                                //добавляем в продукт фразы со ставками
                                foreach (var item in row)
                                {
                                    if (item != null)
                                    {
                                        i = row.IndexOf(item);
                                        BidsItemsList = new List<object>(bidsPageData[r]);
                                        if (item.ToString() != "" && i <= BidsItemsList.Count - 1 && BidsItemsList[i].ToString() != "")
                                        {
                                            if (i > 2) //Фразы и ставки за фразы начинаются с третьего столбца в табличке
                                            {
                                                string bidForProduct = (BidsItemsList[i].ToString() ?? "0") + "000000";
                                                phrasesList.Add(new Phrase(bidForProduct, item.ToString() ?? ""));
                                            }
                                        }

                                        i++;
                                    }
                                }
                                if (phrasesList.Count > 0 && row[0].ToString() != "")
                                    campaignForCreate.AddProduct(new Product(row[0].ToString() ?? "", "35000000", phrasesList));
                            }
                        }
                    }

                    campaignList.Add(campaignForCreate);//добавляем последнюю кампанию в список, которая имеет менее 250 товаров
                }
            }
            else
            {
                MessageBox.Show("No data found PhrasesPageName.");
                return new List<CampaignForCreate>();
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

            //MessageBox.Show(String.Format("Токен будет работать {0} секунд = {1} минут.", token.expires_in.ToString(), token.expires_in / 60));
            return token;

        }
        private async Task<List<CampaignIdFromOzon>> CreateCampaign(List<CampaignForCreate> campaigns, TokenClass token)
        {
            CampaignIdFromOzon cid;
            List<CampaignIdFromOzon> campaignIdList = new List<CampaignIdFromOzon>();
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
                //MessageBox.Show(cid.campaignId);
            }
            return campaignIdList;
        }
        private async void AddProductsInCampaign(CampaignForCreate campaignForCreate, TokenClass token)
        {
            ProductsInCampaignOzon productsInCampaignOzon = new ProductsInCampaignOzon(campaignForCreate);//не передаются Products. Передаются пустые

            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = new HttpResponseMessage();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            request.RequestUri = new Uri("https://performance.ozon.ru:443/api/client/campaign/" + campaignForCreate.campaignId + "/products");
            request.Method = HttpMethod.Post;

            //Пробуем в строке - content-Type пустой
            //var options = new JsonSerializerOptions
            //{
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //    WriteIndented = true
            //};
            //string jsonString = JsonSerializer.Serialize<ProductsInCampaignOzon>(productsInCampaignOzon, options);
            ////Console.WriteLine(jsonString);
            //request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //response = httpClient.PostAsync(request.RequestUri, request.Content).Result;
            //Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            //пробуем в байтах. Если Content-Type пустой - ошибка, тип не определен. Если ставим 
            // byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // то прога зависает
            //var options = new JsonSerializerOptions
            //{
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //    WriteIndented = true
            //};
            //byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes<ProductsInCampaignOzon>(productsInCampaignOzon, options);
            //var byteArrayContent = new ByteArrayContent(jsonBytes);
            ////byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //Console.WriteLine(byteArrayContent.ReadAsStringAsync().Result);
            //response = httpClient.PostAsync(request.RequestUri, byteArrayContent).Result;
            //Console.WriteLine(byteArrayContent.ReadAsStringAsync().Result);

            //Пробуем через httpClient
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            response = await httpClient.PostAsJsonAsync<ProductsInCampaignOzon>("https://performance.ozon.ru:443/api/client/campaign/" + productsInCampaignOzon.campaignId + "/products", productsInCampaignOzon);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show(response.StatusCode.ToString() + "\n" + response.ReasonPhrase + "\n" + response.Content);
            }
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //ClientID
            if (ClientIdTextBox.Text.Length < 32)
            {
                MessageBox.Show("Слишком короткое значение Client Id.");
                return;
            }
            if (!ClientIdTextBox.Text.Contains("@advertising.performance.ozon.ru"))//Неверный формат
            {
                MessageBox.Show("Неверный формат Client Id.");
                return;
            }
            //ClientSecret
            if (ClientSecretTextBox.Text.Length < 32)
            {
                MessageBox.Show("Слишком короткое значение Client Secret.");
                return;
            }
            //SpreadsheetId
            if (SpreadsheetIdTextBox.Text.Length < 20)
            {
                MessageBox.Show("Слишком короткий идентефикатор таблицы");
                return;
            }
            //CampaignName
            if (CampaignNameTextBox.Text.Length < 2)
            {
                MessageBox.Show("Слишком короткое имя кампании. Используйте более одного символа");
                return;
            }
            if (PhrasesPageNameTextBox.Text.Length < 1 || BidsPageNameTextBox.Text.Length < 1)
            {
                MessageBox.Show("Название листов в гугл таблицах не может быть короче 1 символа");
                return;
            }


            ClientID = ClientIdTextBox.Text;
            ClientSecret = ClientSecretTextBox.Text;
            SpreadsheetId = SpreadsheetIdTextBox.Text;
            PhrasesPageName = PhrasesPageNameTextBox.Text;
            BidsPageName = BidsPageNameTextBox.Text;
            CampaignName = CampaignNameTextBox.Text;


            //Получить кампании
            Campaigns = GetData(SpreadsheetId, PhrasesPageName, BidsPageName, CampaignName);
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
                MessageBox.Show(string.Format("Товары распределены по максимум 250 штук в {0} кампаний с именем '{1} [индекс]'", Campaigns.Count, Campaigns[0].title));
            }

            //залить в созданную РК товары
            //разделить получение токена и создане РК
            //создать таймер токена
            //запросить список РК - позже
            //найти РК по названию - позже
            //если нет рк, то предложить создать - позже
            //если есть, то предложить удаление всех товаров из РК? - позже
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("https://docs.google.com/document/d/1RgoFvJecjq2P-ErS_DFdICO0ohjl2RrOD2DH4SawY2g/edit");
        }
    }
}
