using UnityEngine;

namespace Merge;

public class BaseLogger<T> where T : new()
{
	private static T logger;

	private static T instance
	{
		get
		{
			if (logger == null)
			{
				logger = new T();
			}
			return logger;
		}
	}

	public static BaseLogger<T> Instance => instance as BaseLogger<T>;

	public virtual string Key { get; protected set; }

	public virtual bool SendLogs { get; set; }

	public virtual Color? colorKey { get; set; }

	public virtual Color? colorText { get; set; }

	private string KeyText => Key;

	private string ColoredText(string str)
	{
		return str;
	}

	public virtual void _Log(string str)
	{
		if (SendLogs)
		{
			Debug.Log(KeyText + " >>> " + ColoredText(str));
		}
	}

	public void _LogError(string str)
	{
		Debug.Log(KeyText + " >>> " + str.Color(Color.red));
	}

	public virtual void _LogFormat(string str, params object[] args)
	{
		if (SendLogs)
		{
			Debug.Log(KeyText + " >>> " + string.Format(ColoredText(str), args));
		}
	}

	public static void Log(string str)
	{
		Instance._Log(str);
	}

	public static void LogError(string str)
	{
		Instance._LogError(str);
	}

	public static void LogFormat(string str, params object[] args)
	{
		Instance._LogFormat(str, args);
	}
}
