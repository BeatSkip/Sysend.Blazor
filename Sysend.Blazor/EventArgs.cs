using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Sysend.Blazor
{
	public class SysendConnectedEventArgs : EventArgs
	{
		public string UID { get; set; }
		public bool IsPrimary { get; set; }
	}

}
