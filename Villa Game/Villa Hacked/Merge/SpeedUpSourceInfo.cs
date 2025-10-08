using System;
using UnityEngine;

namespace Merge;

[Serializable]
public class SpeedUpSourceInfo
{
	[SerializeField]
	private string key;

	[SerializeField]
	private float multiplier;

	[SerializeField]
	private RefDateTime endTime;

	public string Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public float Multiplier
	{
		get
		{
			return multiplier;
		}
		set
		{
			multiplier = value;
		}
	}

	public RefDateTime EndTime
	{
		get
		{
			return endTime;
		}
		set
		{
			endTime = value;
		}
	}

	public SpeedUpSourceInfo(string key, float multiplier, RefDateTime endTime)
	{
		this.key = key;
		this.multiplier = multiplier;
		this.endTime = endTime;
	}
}
