using Microsoft.JSInterop;

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
		private IJSObjectReference Sysend;
		private readonly Lazy<Task<IJSObjectReference>> jsTask;
		private readonly Lazy<Task<IJSObjectReference>> SysendTask;

		public SysendJs(IJSRuntime jsRuntime)
		{
			jsTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
				"import", "./_content/Sysend.Blazor/sysend.js/sysend.js").AsTask());

			SysendTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
				"import", "./_content/Sysend.Blazor/SysendJsInterop.js").AsTask());

			Tracks = new Dictionary<string, Action<string>>();
		}

		private Dictionary<string, Action<string>> Tracks;

		public async Task OpenAsync()
		{
			Console.WriteLine($"-- openasync()");
			var moduled = await SysendTask.Value;
			var handler = DotNetObjectReference.Create(this);
			await moduled.InvokeVoidAsync("openService", handler);
		}

		public async Task<List<(string id, bool primary)>> GetWindows()
		{
			var module = await jsTask.Value;

			return await Sysend.InvokeAsync<List<(string id, bool primary)>> ("list");
			//return await module.InvokeAsync<List<string>>("list");
		}

		public async Task RegisterTrack(Action<string> Callback)
		{
			Guid g = Guid.NewGuid();

		}

		public async ValueTask<string> Prompt(string message)
		{
			var module = await SysendTask.Value;
			return await module.InvokeAsync<string>("showPrompt", message);
		}

		public async ValueTask<IJSObjectReference> GetSysend()
		{
			var module = await SysendTask.Value;
			return await module.InvokeAsync<IJSObjectReference>("GetSysend");
		}

		public async ValueTask DisposeAsync()
		{
			if (SysendTask.IsValueCreated)
			{
				var module = await SysendTask.Value;
				await module.DisposeAsync();	
			}

			//if (SysendTask.IsValueCreated)
			//{
			//	var sysend = await SysendTask.Value;
			//	await sysend.DisposeAsync();
			//}
		}

		[JSInvokable]
		public async void HandleOnInputReport()
		{
			//this.Sysend = sys;
			Console.WriteLine($"Sysend is initialized!");
			this.Sysend = await GetSysend();
			Console.WriteLine($"Sysend in dotnet space!");
		}

		

		
	}
}