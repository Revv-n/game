using System;
using System.Text;
using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "TemporalItem", menuName = "StripClub/Items/TemporalItem", order = 1)]
public class TemporalItemInfo : NamedScriptableItemInfo
{
	[Tooltip("DD.HH:MM:SS.sss")]
	[SerializeField]
	private string durationString = "0.00:00:00.0";

	private TimeSpan duration;

	private string format;

	private const string prefix = "content.items.temporal.";

	public TimeSpan Duration
	{
		get
		{
			return duration;
		}
		private set
		{
			duration = value;
			format = GetFormat(duration);
		}
	}

	protected override string GetKey()
	{
		return "content.items.temporal." + key;
	}

	private void Awake()
	{
		if (format == null)
		{
			format = GetFormat(Duration);
		}
	}

	public override void OnValidate()
	{
		base.OnValidate();
		Duration = TimeSpan.Parse(durationString);
	}

	public static string GetFormat(TimeSpan duration)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (duration.Days > 0)
		{
			stringBuilder.Append("%d\\d\\ ");
		}
		if (duration.Hours > 0)
		{
			stringBuilder.Append("h\\h\\ ");
		}
		if (duration.Minutes > 0)
		{
			stringBuilder.Append("mm\\m\\ ");
		}
		if (duration.Seconds > 0)
		{
			stringBuilder.Append("ss\\s");
		}
		return stringBuilder.ToString();
	}
}
