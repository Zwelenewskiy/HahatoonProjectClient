using System;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace Client
{
    struct Report
    {
        public int Command;
        public string Id, P, P1, P2;

        public Report(int com, string id, string p, string p1, string p2)
        {
            Command = com;
            Id = id;
            P = p;
            P1 = p1;
            P2 = p2;
        }
    }

    /// <summary> 
    /// Определяет методы для работы клиента 
    /// </summary> 
    class Client
    {
        private int id = -1;
        private HttpWebRequest req;
        private HttpWebResponse resp;

        public Client()
        {
            //Random rand = new Random(); 
            //id = rand.Next(1000); 
        }
        ~Client(){}

        /// <summary> 
        /// Создает запрос к серверу 
        /// </summary> 
        /// <param name="host">Адрес сервера</param> 
        public void Request(string host, string text)
        {
            try
            {
                req = (HttpWebRequest)HttpWebRequest.Create(host);//запрос 
                req.Method = "POST";

                byte[] dataArray = Encoding.UTF8.GetBytes(text);
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = dataArray.Length;

                try
                {
                    using (Stream dataStream = req.GetRequestStream())
                    {
                        dataStream.Write(dataArray, 0, dataArray.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary> 
        /// Возвращает ответ сервера 
        /// </summary> 
        public void Response(ref string answer)
        {
            try
            {
                resp = (HttpWebResponse)req.GetResponse();//ответ 
            }
            catch (WebException ex )
            {
                Console.WriteLine(ex);
            }

            try
            {
                using (StreamReader stream = new StreamReader(
                resp.GetResponseStream(), Encoding.UTF8))
                { 
                    answer = stream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void SetId(int ID)
        {
            id = ID;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            const string HOST = "http://localhost:8888/connection/";
            string answer = null;
            string Id = null;

            Client client = new Client();
            
            while (true)
            {
                if (Id == null)
                {
                    client.Request(HOST, JsonConvert.SerializeObject(new Report(0, "", "", "", "")));
                    client.Response(ref Id);
                    client.SetId(Convert.ToInt32(Id));

                    Console.WriteLine("Ваш ID - " + Id);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Console.WriteLine("1 - отправить сообщение");
                Console.WriteLine("2 - отправить отчет");
                Console.WriteLine("3 - получить последний отчет");
                answer = Console.ReadLine();
                if (answer.ToLower() == "exit")
                {
                    client.Request(HOST, JsonConvert.SerializeObject(new Report(4, Id, "", "", "")));
                    return;
                }
                    
                Byte ans = 0;
                if (answer != "")
                    ans = Convert.ToByte(answer);                   

                 string tmp;
                switch (ans)
                {
                    case 1:
                        Console.Write("Введите текст: ");
                        tmp = Console.ReadLine();
                        if (tmp.ToLower() == "exit")
                        {
                            client.Request(HOST, JsonConvert.SerializeObject(new Report(4, Id, "", "", "")));
                            return;
                        }
                        if (tmp != "")
                        {
                            client.Request(HOST, JsonConvert.SerializeObject(new Report(1, Id, tmp, "", "")));
                            client.Response(ref answer);
                            Console.WriteLine(answer);
                        }
                        Console.WriteLine();
                        break;

                    case 2:
                        client.Request(HOST, JsonConvert.SerializeObject(new Report(2, Id, "1a", "2b", "3c")));
                        client.Response(ref answer);
                        Console.WriteLine(answer);

                        Console.WriteLine();
                        Console.Read();
                        break;

                    case 3:
                        client.Request(HOST, JsonConvert.SerializeObject(new Report(3, Id, "", "", "")));

                        Console.WriteLine("Последний отчет: ");
                        client.Response(ref answer);

                        Report NewReport = JsonConvert.DeserializeObject<Report>(answer);
                        Console.WriteLine("1: " + NewReport.P);
                        Console.WriteLine("2: " + NewReport.P1);
                        Console.WriteLine("3: " + NewReport.P2);

                        Console.WriteLine();
                        Console.Read();
                        break;
                }

                Console.Clear();
            }
        }
    }
}
