﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using CommandLine;
using NFive.PluginManager.Modules;
using Console = Colorful.Console;

namespace NFive.PluginManager
{
	/// <summary>
	/// Application entry-point.
	/// </summary>
	public static class Program
	{
		public const string DefinitionFile = "plugin.yml";
		public const string LockFile = "plugin.lock";
		public const string ResourceFile = "__resource.lua";
		public const string PluginPath = "plugins";
		public const string ConfigurationPath = "config";

		/// <summary>
		/// Application entry-point.
		/// </summary>
		/// <param name="args">The application arguments.</param>
		/// <returns>Exit status code.</returns>
		public static int Main(string[] args)
		{
			try
			{
				return Parser
					.Default
					.ParseArguments<Init, Install>(args)
					.MapResult(
						(Init i) => i.Main(),
						(Install i) => i.Main(),
						e => Task.FromResult(1)
					)
					.GetAwaiter()
					.GetResult();
			}
			catch (Exception ex)
			{
				Console.WriteLine("An unhandled application error has occured:", Color.Red);
				Console.WriteLine(ex.Message);
				if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);

				return 1;
			}
		}
	}
}