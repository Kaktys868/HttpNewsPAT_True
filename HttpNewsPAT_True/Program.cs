using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace HttpNewsPAT_True
{
    public class Program
    {
        public static string url = "https://kupiprodai.ru/";
        static void Main(string[] args)
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

                    CookieContainer cookieContainer = SingIn("student", "Asdfg123");

                    Uri uri = new Uri(url);
                    CookieCollection allCookies = cookieContainer.GetCookies(uri);

                    GetContent(cookieContainer);
                }
                else if (x == "2")
                {
                    Console.WriteLine("1. Ссылка на фото(необязательно)." +
                    "\n2. Название." +
                    "\n3. Описание.");
                    for (int i = 0; i < 3; i++)
                    {
                        y = Console.ReadLine();
                        d[i] = y;
                    }
                    Add(d);
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Enter);
        }

        public static void Add(string[] d)
        {

        }

        public static CookieContainer SingIn(string Login, string Password)
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
                Console.WriteLine(responseFromServer);
                return request.CookieContainer;
            }
        }
        public static string GetContent(CookieContainer cookieContainer)
        {
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookieContainer;  // Используем весь контейнер

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
                string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseFromServer;
            }
        }
        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));
            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText;
                var description = DivNews.ChildNodes[5].InnerText;
                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n");
            }
        }
    }
}
