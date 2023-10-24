using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnetClient.Event
{
		internal class DebugEventArgs : EventArgs
		{
				public string Text { get; set; }

				public DebugEventArgs(string text)
				{
						Text = text;
				}
		}
}
