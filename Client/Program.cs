using System;
using System.Text;
using System.Net;
using System.IO;

namespace Client
{
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
        ~Client()
        {
            //Request("http://localhost:8888/connection/", id.ToString() + " отключился");
        }

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

            Client client = new Client();

            string answer = null;
            string Id = null;
            while (true)
            {
                if (Id == null)
                {
                    client.Request(HOST, "setID");
                    client.Response(ref Id);
                    client.SetId(Convert.ToInt32(Id));

                    Console.WriteLine("Установлен ID - " + Id);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Console.Write("Введите текст: ");
                string tmp = Console.ReadLine();
                if (tmp.ToLower() == "exit")
                {
                    client.Request(HOST, Id + " отключился");
                    return;
                }
                if(tmp != "")
                {
                    client.Request(HOST, Id + " > " + tmp);
                    client.Response(ref answer);
                    Console.WriteLine(answer);
                }                
                Console.WriteLine();
            }
        }
    }
}
