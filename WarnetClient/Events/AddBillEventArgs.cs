using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnetClient.Event
{
		internal class AddBillEventArgs : EventArgs
		{
				public int Seconds { get; set; }

				public AddBillEventArgs(int seconds) { Seconds = seconds; }
		}
}
