using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnetClient.Event
{
		public class ErrorEventArgs : EventArgs
		{
				public string Reason { get; set; }

				public ErrorEventArgs(string reason) { Reason = reason; }
		}
}
