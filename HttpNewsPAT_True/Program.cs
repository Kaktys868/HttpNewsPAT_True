using Fizzler;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace HttpNewsPAT_True
{
    public class parse
    {
        public string Src { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
    }
    public class Program
    {
        public static string url = "https://kupiprodai.ru/";
        public static string s;
        static async Task Main(string[] args)
        {
            string y;
            string[] d = new string[3];
            do {
                Console.WriteLine("1. Парсим сайт вот такой: https://kupiprodai.ru/." +
                    "\n\nИЛИ" +
                    "\n\n2. Добавляем на сервер запись.");
                string x = Console.ReadLine();

                if (x == "1")
                {
                    string logFilePath = "otladka.txt";
                    Trace.Listeners.Clear();
                    Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
                    Trace.AutoFlush = true;

                    Cookie cookieContainer = await SingIn("student", "Asdfg123");

                    string htmlCode = await GetContent(cookieContainer);
                    var newsList = ParsingHtml(htmlCode);
                    foreach (var news in newsList)
                    {
                        Console.WriteLine($"\n{news.Name}");
                        /*if (!string.IsNullOrEmpty(news.Description))
                        {
                            Console.WriteLine($"{news.Description}");
                        }*/
                    }
                }
                else if (x == "2")
                {
                    Cookie cookieContainer = await SingIn("admin", "admin", "http://10.111.20.114/ajax/login.php");
                    Console.WriteLine("1. Ссылка на фото(необязательно)." +
                    "\n2. Название." +
                    "\n3. Описание.");
                    for (int i = 0; i < 3; i++)
                    {
                        y = Console.ReadLine();
                        d[i] = y;
                    }
                    Add(d, cookieContainer);
                    Console.WriteLine(s);
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        }
        public static async Task<bool> Add(string[] d, Cookie token)
        {
            string url = "http://10.111.20.114/ajax/add.php";

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };
            cookieContainer.Add(new Uri(url), new System.Net.Cookie(token.Name, token.Value, token.Path, token.Domain));

            using (var client = new HttpClient(handler))
            {
                var postData = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("src", d[0]),
                        new KeyValuePair<string, string>("name", d[1]),
                        new KeyValuePair<string, string>("description", d[2])
                    });

                var response = await client.PostAsync(url, postData);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        public static async Task<Cookie> SingIn(string Login, string Password, string url = "https://kupiprodai.ru/")
        {
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();

            string postData = $"login={Login}&password={Password}";
            byte[] Data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = Data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(Data, 0, Data.Length);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
                string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var cookies = request.CookieContainer.GetCookies(new Uri(url));
                var token = cookies["token"];

                if (token != null)
                {
                    return new Cookie(token.Name, token.Value, token.Path, token.Domain);
                }
            }
            return null;
        }
        public static async Task<string> GetContent(Cookie cookieContainer)
        {
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
                string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseFromServer;
            }
        }
        public static List<parse> ParsingHtml(string htmlCode)
        {
            List<parse> newsList = new List<parse>();
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);

            var Document = html.DocumentNode;
            IEnumerable<HtmlNode> DivsNews = Document.SelectNodes("//div[@class='gallery_item']");

            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText.Trim();
                //var description = DivNews.ChildNodes[5].InnerText.Trim();

                newsList.Add(new parse { Src = src, Name = name
                    //, Description = description
                    });
            }
            return newsList;
        }
    }
}
