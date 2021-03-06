﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NFive.PluginManager.Extensions;
using NFive.SDK.Plugins.Models;
using Repository = NFive.SDK.Plugins.Models.Repository;
using Version = NFive.SDK.Plugins.Models.Version;

namespace NFive.PluginManager.Adapters
{
	/// <inheritdoc />
	/// <summary>
	/// Download adapter for fetching local plugins.
	/// </summary>
	/// <seealso cref="T:NFive.PluginManager.Adapters.IDownloadAdapter" />
	public class LocalAdapter : IDownloadAdapter
	{
		private readonly Name name;
		private readonly Repository repo;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalAdapter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="repo">The repo.</param>
		public LocalAdapter(Name name, Repository repo)
		{
			this.name = name;
			this.repo = repo;
		}

		/// <inheritdoc />
		/// <summary>
		/// Gets the valid release versions.
		/// </summary>
		/// <exception cref="T:System.IO.FileNotFoundException">Unable to find definition file</exception>
		public async Task<IEnumerable<Version>> GetVersions()
		{
			var path = Path.Combine(Environment.CurrentDirectory, this.repo.Path, Program.DefinitionFile);

			if (!File.Exists(path)) throw new FileNotFoundException("Unable to find definition file", path);

			var definition = Definition.Load(path);

			if (definition.Version == null) return new List<Version> { new Version("latest") }; // TODO: Default version?

			return await Task.FromResult(new List<Version> { definition.Version });
		}

		/// <inheritdoc />
		/// <summary>
		/// Downloads and unpacks the specified plugin version.
		/// </summary>
		/// <param name="version">The version to download.</param>
		public async Task Download(Version version)
		{
			var src = Path.Combine(Environment.CurrentDirectory, this.repo.Path);
			var dst = Path.Combine(Environment.CurrentDirectory, Program.PluginPath, ".staging", this.name.Vendor, this.name.Project);

			new DirectoryInfo(src).Copy(dst);

			await Task.FromResult(0);
		}
	}
}
