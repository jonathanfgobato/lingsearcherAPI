using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using HtmlAgilityPack;

namespace AliexpressScrapy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Digite o nome da loja");
            string store = Console.ReadLine();

            Console.WriteLine("Digite o id do produto");
            string productId = Console.ReadLine();

            Dictionary<string, string> dictStore = new Dictionary<string, string>();
            List<string> listStores = new List<string>() { "aliexpress", "dealextreme"};

            if (!listStores.Contains(store))
            {
                Console.WriteLine("ERROR - Loja não cadastrada");
                Console.WriteLine("Digite qualquer tecla para continuar...");
                Console.ReadKey();
                return;
            }
            dictStore = (ConfigurationManager.GetSection($"StoreSettings/{store}") as System.Collections.Hashtable)
                .Cast<System.Collections.DictionaryEntry>()
                .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            string url = $"{dictStore["url"]}{productId}.html";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string data = String.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(data);

            string fullName = doc.DocumentNode.SelectSingleNode(dictStore["productName"]).InnerText;
            string minPrice = doc.DocumentNode.SelectSingleNode(dictStore["minPrice"]).InnerText;
            //string priceRange = doc.DocumentNode.SelectSingleNode(dictAliexpress["priceRange"]).InnerText;
            //string currency = doc.DocumentNode.SelectSingleNode(dictStore["currency"]).InnerText;

            Console.WriteLine($"Produto: {fullName}");
            Console.WriteLine($"Preço minimo: {minPrice}");
            //Console.WriteLine($"Faixa de preco {priceRange}");
            //Console.WriteLine($"Unidade monetaria: {currency}");
            Console.WriteLine($"Url: {url}");
            Console.WriteLine("Digite qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}

