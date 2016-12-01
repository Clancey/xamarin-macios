﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class CreateDebugConfigurationTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public bool DebugOverWiFi { get; set; }

		public string DebugIPAddresses { get; set; }

		[Required]
		public string DebuggerPort { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		public override bool Execute ()
		{
			Log.LogTaskName ("CreateDebugConfiguration");
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("DebugOverWiFi", DebugOverWiFi);
			Log.LogTaskProperty ("DebugIPAddresses", DebugIPAddresses);
			Log.LogTaskProperty ("DebuggerPort", DebuggerPort);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);

			var ips = DebugIPAddresses?.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			var path = Path.Combine (AppBundleDir, "MonoTouchDebugConfiguration.txt");
			var added = new HashSet<string> ();
			var builder = new StringBuilder ();

			if (ips != null) {
				foreach (var ip in ips) {
					if (added.Contains (ip))
						continue;

					builder.Append ("IP: ");
					builder.AppendLine (ip);
					added.Add (ip);
				}
			}

			if (!DebugOverWiFi && !SdkIsSimulator)
				builder.AppendLine ("USB Debugging: 1");

			builder.Append ("Port: ");
			builder.AppendLine (DebuggerPort);

			var text = builder.ToString ();

			try {
				if (!File.Exists (path) || File.ReadAllText (path) != text)
					File.WriteAllText (path, text);
			} catch (Exception ex) {
				Log.LogError (ex.Message);
			}

			return !Log.HasLoggedErrors;
		}
	}
}
