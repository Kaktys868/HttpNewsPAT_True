using System;
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
        public static string url = "";
        static void Main(string[] args)
        {
            CookieContainer cookieContainer = SingIn("student", "Asdfg123");

            Uri uri = new Uri(url);
            CookieCollection allCookies = cookieContainer.GetCookies(uri);

            Console.WriteLine("\n=== ВСЕ ПОЛУЧЕННЫЕ КУКИ ===");
            foreach (Cookie cookie in allCookies)
            {
                Console.WriteLine($"Имя: {cookie.Name}");
                Console.WriteLine($"Значение: {cookie.Value}");
                Console.WriteLine($"Домен: {cookie.Domain}");
                Console.WriteLine($"Путь: {cookie.Path}");
                Console.WriteLine("---");
            }

            GetContent(cookieContainer);

            Console.Read();
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
                Console.WriteLine("Ответ: " + responseFromServer);
                return responseFromServer;
            }
        }
    }
}
