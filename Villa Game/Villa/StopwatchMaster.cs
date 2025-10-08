using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class StopwatchMaster
{
	private static Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();

	public static void Start(string key)
	{
		if (!stopwatches.ContainsKey(key))
		{
			stopwatches[key] = new Stopwatch();
		}
		stopwatches[key].Start();
	}

	public static TimeSpan Stop(string key, string text = null, Action<string> callback = null, bool remove = true)
	{
		if (stopwatches.TryGetValue(key, out var value) && value != null && value.IsRunning)
		{
			value.Stop();
			if (callback != null)
			{
				if (text == null)
				{
					text = key;
				}
				if (!text.Contains("{0}"))
				{
					text += " {0}";
				}
				callback(string.IsNullOrEmpty(text) ? value.Elapsed.ToString() : string.Format(text, value.Elapsed));
			}
			if (remove)
			{
				stopwatches.Remove(key);
			}
			return value.Elapsed;
		}
		return TimeSpan.Zero;
	}
}
