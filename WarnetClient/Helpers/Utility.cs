using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WarnetClient
{
		internal static class Utility
		{
				public static string GetIPAdress()
				{
						return Dns.GetHostEntry(Environment.MachineName).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
				}
		}
				
}
