﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NFive.PluginManager.Models;

namespace NFive.PluginManager
{
	/// <summary>
	/// Represent and generate a FiveM Lua resource file.
	/// </summary>
	public static class ResourceGenerator
	{
		/// <summary>
		/// Gets or sets the FiveM resource manifest version.
		/// </summary>
		/// <value>
		/// The resource manifest version.
		/// </value>
		public static Guid ManifestVersion { get; set; } = new Guid("44febabe-d386-4d18-afbe-5e627f4af937");

		/// <summary>
		/// Gets the default sever scripts.
		/// </summary>
		/// <value>
		/// The default sever scripts.
		/// </value>
		public static IEnumerable<string> DefaultSeverScripts => new List<string>
		{
			"NFive.Server.net.dll"
		};

		/// <summary>
		/// Gets the default client scripts.
		/// </summary>
		/// <value>
		/// The default client scripts.
		/// </value>
		public static IEnumerable<string> DefaultClientScripts => new List<string>
		{
			"NFive.Client.net.dll"
		};

		/// <summary>
		/// Gets the default client files.
		/// </summary>
		/// <value>
		/// The default client files.
		/// </value>
		public static IEnumerable<string> DefaultClientFiles => new List<string>
		{
			"Newtonsoft.Json.dll",
			"System.ComponentModel.DataAnnotations.dll",

			"NFive.SDK.Core.net.dll",
			"NFive.SDK.Client.net.dll"
		};

		/// <summary>
		/// Serializes the specified <see cref="DefinitionGraph"/> to a FiveM Lua resource file format.
		/// </summary>
		/// <param name="graph">The definition graph to serialize.</param>
		/// <returns>The definition graph in FiveM Lua resource file format.</returns>
		public static ResourceString Serialize(DefinitionGraph graph)
		{
			var output = new StringBuilder();

			WriteLine(ref output, "-- FiveM resource definition");
			WriteLine(ref output, "-- This file is automatically generated with `nfpm install` - any manual changes will be lost");
			WriteLine(ref output);

			WriteLine(ref output, "resource_manifest_version", ManifestVersion.ToString());
			WriteLine(ref output);

			WriteLine(ref output, "server_scripts {");
			WriteLine(ref output, "\t-- NFive");
			foreach (var file in DefaultSeverScripts) WriteLine(ref output, $"\t'{file}',");
			WriteLine(ref output, "}");
			WriteLine(ref output);

			WriteLine(ref output, "client_scripts {");
			foreach (var plugin in graph.Definitions.Where(d => d.Client?.Include?.Count > 0 || d.Client?.Main?.Count > 0))
			{
				WriteLine(ref output, $"\t-- {plugin.Name}@{plugin.Version}");
				if (plugin.Client?.Include != null) foreach (var file in plugin.Client.Include) WriteLine(ref output, $"\t'{Path.Combine(Program.PluginPath, plugin.Name.Vendor, plugin.Name.Project, file).Replace(Path.DirectorySeparatorChar, '/')}.net.dll',");
				if (plugin.Client?.Main != null) foreach (var file in plugin.Client.Main) WriteLine(ref output, $"\t'{Path.Combine(Program.PluginPath, plugin.Name.Vendor, plugin.Name.Project, file).Replace(Path.DirectorySeparatorChar, '/')}.net.dll',");
				WriteLine(ref output);
			}
			WriteLine(ref output, "\t-- NFive");
			foreach (var file in DefaultClientScripts) WriteLine(ref output, $"\t'{file}',");
			WriteLine(ref output, "}");
			WriteLine(ref output);

			WriteLine(ref output, "files {");
			WriteLine(ref output, "\t-- NFive");
			foreach (var file in DefaultClientFiles) WriteLine(ref output, $"\t'{file}',");
			foreach (var plugin in graph.Definitions.Where(d => d.Client?.Files?.Count > 0))
			{
				WriteLine(ref output);
				WriteLine(ref output, $"\t-- {plugin.Name}@{plugin.Version}");
				foreach (var file in plugin.Client.Files) WriteLine(ref output, $"\t'{Path.Combine(Program.PluginPath, plugin.Name.Vendor, plugin.Name.Project, file).Replace(Path.DirectorySeparatorChar, '/')}',");
			}
			WriteLine(ref output, "}");

			var loadscreen = graph.Definitions.FirstOrDefault(d => d.Client?.Loadscreen != null);
			if (loadscreen != null)
			{
				WriteLine(ref output);
				WriteLine(ref output, $"-- {loadscreen.Name}@{loadscreen.Version}");
				WriteLine(ref output, "loadscreen", Path.Combine(Program.PluginPath, loadscreen.Name.Vendor, loadscreen.Name.Project, loadscreen.Client.Loadscreen).Replace(Path.DirectorySeparatorChar, '/'));
			}

			var ui = graph.Definitions.FirstOrDefault(d => d.Client?.Ui != null);
			if (ui != null)
			{
				WriteLine(ref output);
				WriteLine(ref output, $"-- {ui.Name}@{ui.Version}");
				WriteLine(ref output, "ui_page", Path.Combine(Program.PluginPath, ui.Name.Vendor, ui.Name.Project, ui.Client.Ui).Replace(Path.DirectorySeparatorChar, '/'));
			}

			return new ResourceString(output.ToString());
		}

		private static void WriteLine(ref StringBuilder builder, string line = "")
		{
			builder.AppendLine(line);
		}

		private static void WriteLine(ref StringBuilder builder, string key, string value)
		{
			WriteLine(ref builder, $"{key} '{value}'");
		}
	}
}
