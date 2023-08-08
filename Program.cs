using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EXE_Detect_IP
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			string IP = GetIpLocal();
			Console.WriteLine(String.Format("IP privada: {0} (Host: {1})", IP, GetHost()));			


			string IpA = GetIpCualEsMiIP();
			Console.WriteLine("IP pública según cual-es-mi-ip.net: " + IpA);


			string IpB = GetIpDydns();
			Console.WriteLine("IP pública según dyndns.org: " + IpB);

			string IpC = GetIpIfconfig();
			Console.WriteLine("IP pública según ifconfig.me: " + IpC);

			string IpD = GetIpCualEsDireccionMiIP();
			Console.WriteLine("IP pública según cual-es-mi-direccion-ip.com: " + IpD);

			String IpE = await GetIpMiIp();
			Console.WriteLine("IP pública según miip.es: " + IpE);

			string IpF = await GetIpIpinfo();
			Console.WriteLine("IP pública según ipinfo.io: " + IpF);

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
				int last = address.LastIndexOf("</span>");
				address = address.Substring(first, last - first);
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
				WebRequest request = WebRequest.Create(url);
				using (WebResponse response = request.GetResponse())
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

			HttpClient client = new HttpClient();
			Task<string> getStringTask = client.GetStringAsync(url);

			String urlContents = await getStringTask;

			return urlContents;
		}

	}
}
