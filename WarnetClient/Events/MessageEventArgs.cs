using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnetClient.Event
{
		public class MessageEventArgs : EventArgs
		{
				public string Type { get; set; }

				public MessageEventArgs(string type) { Type = type; }
		}
}
