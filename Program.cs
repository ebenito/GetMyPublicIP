using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EXE_Detect_IP
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			string IP = GetIpLocal();
			Console.WriteLine(String.Format("IP privada: {0} (Host: {1})", IP, GetHost()));
			Console.WriteLine("");

			string IpF = await GetIpIpinfo();
			Console.WriteLine("IP pública según ipinfo.io (el más confiable): " + IpF);

			//string IpH = GetIpCdMon();
			//Console.WriteLine("IP pública según cdmon.com: " + IpH);

            string IpG = await GetIpIONOS();
			Console.WriteLine("IP pública según IONOS.es: " + IpG);

			string IpA = GetIpCualEsMiIP();
			Console.WriteLine("IP pública según cual-es-mi-ip.net: " + IpA);

			string IpB = GetIpDydns();
			Console.WriteLine("IP pública según dyndns.org: " + IpB);

			string IpC = GetIpIfconfig();
			Console.WriteLine("IP pública según ifconfig.me: " + IpC);

			string IpD = GetIpCualEsDireccionMiIP();
			Console.WriteLine("IP pública según cual-es-mi-direccion-ip.com: " + IpD);

			string IpE = await GetIpMiIp();
			Console.WriteLine("IP pública según miip.es: " + IpE);




			Console.ReadKey();
		}

		static string GetIpLocal()
		{
			string IPAddress = "";
			IPHostEntry Host = default(IPHostEntry);
			string Hostname = null;
			Hostname = System.Environment.MachineName;
			Host = Dns.GetHostEntry(Hostname);
			foreach (IPAddress IP in Host.AddressList)
			{
				if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					IPAddress = Convert.ToString(IP);
				}
			}

			return IPAddress;
		}

		static string GetHost() 
		{
			//https://www.dotnetstuffs.com/get-system-ip-address-using-c-sharp/
			string myHost = Dns.GetHostName();
			return myHost;
		}

		static string GetIpCualEsMiIP()
		{
			String address = "";

			try
			{ 	
				address = DownloadHtml("https://www.cual-es-mi-ip.net/", "CualEsMiIP.html");

				int first = address.IndexOf("Tu dirección IP es") + 53;
				address = address.Substring(first, 15);

				string ipPattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
				Match ipMatch = Regex.Match(address, ipPattern);

				if (ipMatch.Success)
				{
					address = ipMatch.Value;
				}
				else
				{
					address = "No se encontró ninguna dirección IP.";
				}

			}
			catch (Exception ex)
			{ 
				Console.WriteLine(ex.Message);

				address = "No se ha podido obtener la IP desde este servidor.";
			}


			return address;
		}

		static string GetIpDydns()
		{
			String address = "";
			address = DownloadWebResult("http://checkip.dyndns.org/");
									
			try
			{
				if (!string.IsNullOrEmpty(address))
				{
					int first = address.IndexOf("Address: ") + 9;
					int last = address.LastIndexOf("</body>");
					address = address.Substring(first, last - first);
				}
				else
				{
					address = "No se ha podido obtener la IP desde este servidor.";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				address = "No se ha podido obtener la IP desde este servidor.";
			}

			return address;
		}

        static string GetIpCdMon()
        {
            String address = "";
           
			           
            try
            {
				address = DownloadWebResult("https://www.cdmon.com/es/apps/ip");              

                // Procesar el HTML:
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(address);

                // Buscar por la clase del elemento que contiene la dirección IP
                var ipNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'ip_ip-container__Diy4K')]");

                if (ipNode != null)
                {
                    address = ipNode.InnerText.Trim();
                }
                else
                {
                    address = "No se pudo encontrar la dirección IP.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                address = "No se ha podido obtener la IP desde este servidor.";
            }


            return address;
        }

        static string GetIpIfconfig()
		{
			String address = "";	
			address = DownloadWebResult("https://ifconfig.me/ip");

			return address;
		}

		static string GetIpCualEsDireccionMiIP()
		{
			String address = "";
			address = DownloadHtml("https://www.cual-es-mi-direccion-ip.com/", "CualEsDireccionMiIP.html");

			if (!string.IsNullOrEmpty(address))
			{
				int first = address.IndexOf("/ip-info/") + 9;
				address = address.Substring(first);

				int last = address.IndexOf(">") - 1;
				address = address.Substring(0, last);
			}
			else
			{
				address = "No se ha podido obtener la IP desde este servidor.";
			}

			return address;
		}


		private static async Task<string> GetIpMiIp() //con métodos asincronos
		{
			String address = "";
			address = await DownloadWebResultAsync("https://miip.es/");

			if (!string.IsNullOrEmpty(address))
			{
				int first = address.IndexOf("Tu IP es") + 9;
				address = address.Substring(first);

				int last = address.IndexOf("</h2>") ;
				address = address.Substring(0, last);
			}
			else
			{
				address = "No se ha podido obtener la IP desde este servidor.";
			}

			return address;
		}


		private static async Task<string> GetIpIONOS() //con métodos asincronos
		{
			String address = "";
			//address = await DownloadWebResultAsync("https://www.ionos.es/tools/direccion-ip");

			//if (!string.IsNullOrEmpty(address))
			//{
			//	int first = address.IndexOf("<div class=\"text-md-center text-lg-center heading-1 ml-md-0 ml-lg-0 mw-lg-10 mw-md-10 mx-md-auto mx-lg-auto pt-12\">") + 117;
			//	address = address.Substring(first);

			//	int last = address.IndexOf("</div>");
			//	address = address.Substring(0, last);
			//}
			//else
			//{
			//	address = "No se ha podido obtener la IP desde este servidor.";
			//}

			var url = "https://www.ionos.es/tools/direccion-ip";
			var httpClient = new HttpClient();

			// Configurar encabezados de solicitud
			httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
			httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			httpClient.DefaultRequestHeaders.Add("Accept-Language", "es-ES,es;q=0.5");

			try
			{
				var response = await httpClient.GetAsync(url);
				var html = await response.Content.ReadAsStringAsync();

				// Procesar el HTML:
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(html);

				// Buscar por la clase del elemento que contiene la dirección IP
				var ipNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'heading-1')]");

				if (ipNode != null)
				{
					address = ipNode.InnerText.Trim();
				}
				else
				{
					address = "No se pudo encontrar la dirección IP.";
				}
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nExcepción capturada!");
				Console.WriteLine("Mensaje :{0} ", e.Message);
			}			


			return address;
		}


		private static async Task<string> GetIpIpinfo()
		{
			String address = "";
			address = await DownloadWebResultAsync("https://ipinfo.io/ip");
			
			return address;
		}


		static string DownloadWebResult(string url)
		{
			String Resultado = "";
			try
			{
				//WebRequest request = WebRequest.Create(url);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.945.0 Safari/537.36 Edg/93.0.961.0";
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                httpWebRequest.Headers.Add("Accept-Language", "es-ES,es;q=0.5");
				httpWebRequest.Referer = "https://www.bing.com/";

                using (WebResponse response = httpWebRequest.GetResponse())
				using (StreamReader stream = new StreamReader(response.GetResponseStream()))
				{
					Resultado = stream.ReadToEnd();
				}
			}
			catch (Exception ex)
			{ 
				Console.WriteLine(ex.Message);

				Resultado = "No se ha podido obtener la IP desde este servidor.";
			}

			return Resultado;
		}

		static string DownloadHtml(string url, string file ="")
		{
			string htmlCode = "";
			using (WebClient client = new WebClient()) 
			{
				if (file != "")
				{
					string path = System.IO.Directory.GetCurrentDirectory() + "\\" + file;
					client.DownloadFile(url,path);
					htmlCode = File.ReadAllText(path);
					File.Delete(path);
				}
				else
				{
					// O directamente, sin descargarlo:
					htmlCode = client.DownloadString(url);
				}
				
			}

			return htmlCode;
		}


		static async Task<string> DownloadWebResultAsync(string url)
		{

			try
			{
				HttpClient client = new HttpClient();
				Task<string> getStringTask = client.GetStringAsync(url);

				String urlContents = await getStringTask;

				return urlContents;
			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}
