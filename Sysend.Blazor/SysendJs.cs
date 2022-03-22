using Microsoft.JSInterop;
using System.Diagnostics;

namespace Sysend.Blazor
{
	// This class provides an example of how JavaScript functionality can be wrapped
	// in a .NET class for easy consumption. The associated JavaScript module is
	// loaded on demand when first needed.
	//
	// This class can be registered as scoped DI service and then injected into Blazor
	// components for use.

	public class SysendJs : IAsyncDisposable
	{
		private readonly Lazy<Task<IJSObjectReference>> SysendTask;
		private readonly IJSRuntime jsRuntime;

		public Dictionary<string, string> Clients { get; private set; }

		private string ClientIdentifier;
		private string partialguid;

		public string UID { get; private set; }

		public SysendJs(IJSRuntime jsruntime)
		{
			jsRuntime = jsruntime;
			SysendTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
				"import", "./_content/Sysend.Blazor/SysendJsInterop.js").AsTask());

			SysendConnected += OnSysteakljasdf;
			Clients = new Dictionary<string, string>();
			partialguid = Guid.NewGuid().ToString();
			Console.WriteLine($"myguid: {partialguid}");
			partialguid = partialguid.Split('-').First().Substring(1);
			Console.WriteLine($"myguid: {partialguid}");
		}

		public async Task UpdateIdentifier(string identifier)
		{
			this.ClientIdentifier = identifier.Replace("\t", " ") + "-" + partialguid;
			await Broadcast("sysend_id", "refresh");
			await Broadcast("sysend_id", getIdentifierbroadcast());
		}

		public List<string> GetClients()
		{
			return Clients.Keys.ToList();
		}

		#region Events

		public EventHandler ClientsUpdated;

		private void onClientUpdated()
		{
			ClientsUpdated?.Invoke(this, EventArgs.Empty);
		}
		#endregion


		#region Sysend Interop

		public async Task On(string channel, Func<string, string, Task> callback)
		{
			await jsRuntime.InvokeVoidAsync("sysend.on", channel, CallBackInteropWrapper.Create<string, string>(callback));
		}

		public async Task Off(string channel, Func<string, string, Task> callback)
		{
			await jsRuntime.InvokeVoidAsync("sysend.off", channel, CallBackInteropWrapper.Create<string, string>(callback));
		}

		public async Task Broadcast(string channel, string message)
		{
			await jsRuntime.InvokeVoidAsync("sysend.broadcast", channel, message);
		}

		public async Task<List<SysendItem>> List()
		{
			return await jsRuntime.InvokeAsync<List<SysendItem>>("sysend.list");
		}

		public async Task Post(string client, string message)
		{
			await jsRuntime.InvokeVoidAsync("sysend.post", client, message);
		}

		public async Task Post(string client, object data)
		{
			await jsRuntime.InvokeVoidAsync("sysend.post", client, data);
		}

		
		#endregion


		#region Events initialization

		private EventHandler<SysendConnectedEventArgs> SysendConnected;

		private void InteropOnSysendConnected(string id)
		{
			SysendConnected?.Invoke(this, new SysendConnectedEventArgs() { UID = id} );
		}

		private async void OnSysteakljasdf(object sender, SysendConnectedEventArgs e)
		{
			this.UID = e.UID;
			SysendInitialized();
		}

		#endregion

		#region internal initialization

		private async Task CallbackIdentifyBroadcast(string data, string channel)
		{
			if (data == "refresh")
			{
				this.Clients.Clear();
				await Broadcast("sysend_id", getIdentifierbroadcast());
			}
			else
			{
				Console.WriteLine($"data: {data}");
				var x = data.Split("\t");
				if (!Clients.ContainsKey(x[0]))
					Clients.Add(x[0], x[1]);
				else
					Clients[x[0]] = x[1];

				onClientUpdated();
				Console.WriteLine($"Connection opended! {x[0]} | {x[1]}");
			}
			
		}
		
		private string getIdentifierbroadcast()
		{
			return ClientIdentifier + "\t" + UID;
		}

		private async void SysendInitialized()
		{
			await On("sysend_id", CallbackIdentifyBroadcast);
			await Broadcast("sysend_id", "refresh");
			await Broadcast("sysend_id", getIdentifierbroadcast());
			
		}

		public async Task StartAsync(string identifier = "Sysend")
		{
			this.ClientIdentifier = identifier.Replace("\t", " ") + "-" + partialguid;
			var module = await SysendTask.Value;
			var referen = DotNetObjectReference.Create(this);
			await module.InvokeVoidAsync("InitSysend", referen);
		}

		public async ValueTask DisposeAsync()
		{

			if (SysendTask.IsValueCreated)
			{
				var sysend = await SysendTask.Value;
				await sysend.DisposeAsync();
			}
		}

		[JSInvokable]
		public void InteropOnSysendLoaded(string guid)
		{
			InteropOnSysendConnected(guid);
			Console.WriteLine($"sysend initialized with id: {guid}");
			
		}

		#endregion
	}

}