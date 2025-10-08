using System;
using System.Text;
using GreenT.Localizations;

namespace StripClub.Extensions;

public class TimeHelper
{
	private const string dayKey = "general.date_time.day";

	private const string hourKey = "general.date_time.hour";

	private const string minuteKey = "general.date_time.minute";

	private const string daysKey = "general.date_time.days";

	private const string hoursKey = "general.date_time.hours";

	private const string minutesKey = "general.date_time.minutes";

	private const char space = ' ';

	private readonly LocalizationService _localizationService;

	public TimeHelper(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public string UseCombineFormat(TimeSpan timeSpan)
	{
		if (!(timeSpan.TotalHours >= 1.0))
		{
			return ToShortFormat(timeSpan);
		}
		return ToFormattedString(timeSpan, longForm: false);
	}

	public string ToShortFormat(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return timeSpan.ToString("dd\\:hh");
		}
		return timeSpan.ToString((timeSpan.Hours > 0) ? "hh\\:mm" : "mm\\:ss");
	}

	public string ToFormattedString(TimeSpan timeSpan, bool longForm = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (longForm)
		{
			AddPeriodToLongFormat(timeSpan.Days, _localizationService.Text("general.date_time.day"), _localizationService.Text("general.date_time.days"), stringBuilder);
			if (timeSpan.Days > 0 && timeSpan.Hours > 0)
			{
				stringBuilder.Append(' ');
			}
			AddPeriodToLongFormat(timeSpan.Hours, _localizationService.Text("general.date_time.hour"), _localizationService.Text("general.date_time.hours"), stringBuilder);
			if (timeSpan.Days < 1)
			{
				if (timeSpan.Hours > 0 && timeSpan.Minutes > 0)
				{
					stringBuilder.Append(' ');
				}
				AddPeriodToLongFormat(timeSpan.Minutes, _localizationService.Text("general.date_time.minute"), _localizationService.Text("general.date_time.minutes"), stringBuilder);
			}
		}
		else
		{
			AddPeriodToShortFormat(timeSpan.Days, _localizationService.Text("general.date_time.day")[0], stringBuilder);
			if (timeSpan.Days > 0 && timeSpan.Hours > 0)
			{
				stringBuilder.Append(' ');
			}
			AddPeriodToShortFormat(timeSpan.Hours, _localizationService.Text("general.date_time.hour")[0], stringBuilder);
			if (timeSpan.Days < 1)
			{
				if (timeSpan.Hours > 0 && timeSpan.Minutes > 0)
				{
					stringBuilder.Append(' ');
				}
				AddPeriodToShortFormat(timeSpan.Minutes, _localizationService.Text("general.date_time.minute")[0], stringBuilder);
			}
		}
		if (timeSpan.TotalMinutes < 1.0)
		{
			stringBuilder.Append(timeSpan.Seconds);
			stringBuilder.Append(' ');
			stringBuilder.Append('s');
		}
		return stringBuilder.ToString();
	}

	private void AddPeriodToLongFormat(int periodValue, string timeUnit, string pluralTime, StringBuilder builder)
	{
		if (periodValue > 0)
		{
			builder.Append(periodValue);
			builder.Append(' ');
			if (periodValue == 1)
			{
				builder.Append(timeUnit);
			}
			else
			{
				builder.Append(pluralTime);
			}
		}
	}

	private void AddPeriodToShortFormat(int value, char symbol, StringBuilder builder)
	{
		if (value > 0)
		{
			builder.Append(value);
			builder.Append(symbol);
		}
	}
}
