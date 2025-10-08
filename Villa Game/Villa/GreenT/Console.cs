using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GreenT.Cheats;
using UniRx;
using UnityEngine;

namespace GreenT;

public static class Console
{
	private const string NO_ELEMENTS = "no elements";

	public static ConsoleLogSettingsSO LogSettings;

	public static event Action<string, IEnumerable, LogType> OnLogCollection;

	public static IObservable<T> Debug<T>(this IObservable<T> source, string label = null, LogType logType = LogType.Game)
	{
		return source.Do(delegate
		{
		});
	}

	[HideInCallstack]
	public static Exception SendException(this Exception innerException, string errMsg)
	{
		return new Exception(errMsg).LogException();
	}

	[HideInCallstack]
	public static Exception LogException(this Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
		return exception;
	}

	public static string CollectionToString<T>(this IEnumerable<T> collection, char delimiter = ',')
	{
		if (!collection.Any())
		{
			return "no elements";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(collection.First());
		foreach (T item in collection.Skip(1))
		{
			stringBuilder.Append(delimiter);
			stringBuilder.Append(' ');
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	[Conditional("CHEATS")]
	[HideInCallstack]
	public static void SendLog(this object sender, string logMsg, LogType type = LogType.Game)
	{
		new StringBuilder(type.ToString() + " " + sender.GetType().Name + ": " + logMsg);
	}

	[Conditional("CHEATS")]
	[HideInCallstack]
	public static void SendLog(string logMsg, LogType type = LogType.Game)
	{
		if ((type | LogType.Error) == type)
		{
			UnityEngine.Debug.LogError(logMsg);
		}
		else if ((type | LogType.Warning) == type)
		{
			UnityEngine.Debug.LogWarning(logMsg);
		}
		else if (IsValidateLog(type))
		{
			UnityEngine.Debug.Log(logMsg);
		}
	}

	private static bool IsValidateLog(LogType type)
	{
		if (LogSettings != null)
		{
			return (LogSettings.Target & type) != 0;
		}
		return false;
	}

	public static void SendLogCollection(this object sender, string text, IEnumerable collection, LogType logType = LogType.Data)
	{
	}

	public static void SendLogCollection(string text, IEnumerable collection, LogType logType = LogType.Data)
	{
	}
}
