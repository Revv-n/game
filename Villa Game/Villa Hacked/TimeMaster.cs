using System;
using System.Globalization;

public static class TimeMaster
{
	public const string FORMAT = "d.M.yyyy HH:mm:ss";

	public static DateTime Now => DateTime.Now;

	public static DateTime Default => default(DateTime);

	public static RefTimer DefaultTimer => new RefTimer(0f, Default);

	public static bool TryParseDateTime(string date, out DateTime result)
	{
		return DateTime.TryParseExact(date, "d.M.yyyy HH:mm:ss", null, DateTimeStyles.None, out result);
	}

	public static RefTimer GetRefTimer(float time)
	{
		return new RefTimer(time, Now);
	}

	public static string FormatDateTime(DateTime date)
	{
		return date.ToString("d.M.yyyy HH:mm:ss");
	}
}
