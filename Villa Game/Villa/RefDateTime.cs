using System;
using UnityEngine;

[Serializable]
public class RefDateTime
{
	[SerializeField]
	private string data;

	private DateTime parsed;

	private bool is_parsed;

	public DateTime Value
	{
		get
		{
			return GetData();
		}
		set
		{
			SetData(value);
		}
	}

	public bool IsDefault => Value == TimeMaster.Default;

	public string StringValue => data;

	public static RefDateTime Now => new RefDateTime(TimeMaster.Now);

	public static RefDateTime Default => new RefDateTime(TimeMaster.Default);

	public RefDateTime()
		: this(TimeMaster.Default)
	{
	}

	public RefDateTime(DateTime date)
	{
		Value = date;
	}

	public void AddTime(float seconds)
	{
		Value = parsed.AddSeconds(seconds);
	}

	public DateTime GetData()
	{
		if (!is_parsed)
		{
			is_parsed = TimeMaster.TryParseDateTime(data, out parsed);
		}
		return parsed;
	}

	public RefDateTime SetData(DateTime value)
	{
		data = TimeMaster.FormatDateTime(value);
		parsed = value;
		is_parsed = true;
		return this;
	}

	public RefDateTime Copy()
	{
		return new RefDateTime
		{
			data = data,
			parsed = parsed,
			is_parsed = is_parsed
		};
	}
}
