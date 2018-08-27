﻿using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using JetBrains.Annotations;
using NFive.PluginManager.Models;
using NFive.PluginManager.Models.Plugin;
using Console = Colorful.Console;

namespace NFive.PluginManager.Modules
{
	/// <summary>
	/// List installed NFive plugins.
	/// </summary>
	[UsedImplicitly]
	[Verb("list", HelpText = "List installed NFive plugins.")]
	internal class List
	{
		internal async Task<int> Main()
		{
			Definition definition;

			try
			{
				definition = Definition.Load();
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Use `nfpm init` to setup NFive in this directory");

				return 1;
			}

			DefinitionGraph graph;
			
			try
			{
				graph = DefinitionGraph.Load();
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Use `nfpm install` to install some dependencies first");

				return 1;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to build definition graph (PANIC):", Color.Red);
				Console.WriteLine(ex.Message);
				if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);

				return 1;
			}
			
			Console.WriteLine($"{definition.FullName}");

			foreach (var graphDefinition in graph.Definitions)
			{
				Console.WriteLine($"+-- {graphDefinition.FullName}");

				if (graphDefinition.DependencyNodes == null) break;

				foreach (var graphDefinitionDependencyNode in graphDefinition.DependencyNodes)
				{
					Console.WriteLine($"| +-- {graphDefinitionDependencyNode.FullName}");
				}
			}

			return await Task.FromResult(0);
		}
	}
}