using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnetClient.Event
{
		public class BroadcastMessage
		{
				public string Type { get; set; }
				public string Content { get; set; }

				public BroadcastMessage() { }

				public BroadcastMessage(string type, string content)
				{
						Type = type;
						Content = content;
				}
		}
}
