﻿using InnerCore.Api.HueSync.Extensions;
using InnerCore.Api.HueSync.Models.Command;
using InnerCore.Api.HueSync.Models.Enum;
using System;
using System.Threading.Tasks;

namespace InnerCore.Api.HueSync.Sample
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("please enter the ip address of your Hue Sync Box:");
			var ip = Console.ReadLine();

			var client = new HueSyncBoxClient(ip);

			// you can validate the connection by calling for device details (no access token needed yet)
			var device = await client.GetDeviceAsync();
			Console.WriteLine($"found the device '{device.Name}'");

			Console.WriteLine("if you already have an access token you can enter it now or press enter to start the registration progress");
			string accessToken = Console.ReadLine();

			if (!string.IsNullOrEmpty(accessToken))
			{
				client.Initialize(accessToken);
			}
			else
			{
				while (string.IsNullOrEmpty(accessToken))
				{
					Console.WriteLine("please press and hold the button on the Hue Sync Box until the led turns green");
					Console.WriteLine("press enter to continue the registration");
					Console.ReadLine();
					accessToken = await client.RegisterAsync("Demo", "MTIzNDU2Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTI=", Environment.MachineName);
				}
				// here you would usually persist the token and for the next app start you would call client.Initialize(accessToken) directly instead of registering
			}

			var state = await client.GetStateAsync();
			Console.WriteLine("Here are some details about your Hue Sync Box:");
			Console.WriteLine($"firmware: {state.Device.FirmwareVersion}");
			Console.WriteLine($"current mode: {state.Execution.Mode}");
			Console.WriteLine($"current input: {state.Execution.HdmiSource}");
			Console.WriteLine($"current content: {state.Hdmi.ContentSpecs}");

			Console.WriteLine("press enter to set the mode to 'game, intense', apply the max brightness and change the source to hdmi 2");
			Console.ReadLine();
			var action = new ExecutionCommand()
				.SetMode(Mode.Game)
				.SetBrightness(200)
				.SetHdmiSource(HdmiSource.Input2);
			await client.ApplyExecutionCommandAsync(action);

			Console.WriteLine("press enter stop syncing");
			Console.ReadLine();
			action = new ExecutionCommand().SetMode(Mode.Passthrough);
			await client.ApplyExecutionCommandAsync(action);

			Console.WriteLine("press enter put the box into standby mode");
			Console.ReadLine();
			action = new ExecutionCommand().SetMode(Mode.PowerSave);
			await client.ApplyExecutionCommandAsync(action);

			Console.WriteLine("press enter to complete this demo");
			Console.ReadLine();
		}
	}
}
