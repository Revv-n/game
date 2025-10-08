using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge;

[Serializable]
public class SpeedUpUnitData
{
	[SerializeField]
	private Dictionary<string, List<SpeedUpSourceInfo>> sources = new Dictionary<string, List<SpeedUpSourceInfo>>();

	[SerializeField]
	private RefDateTime becomesOffline;

	public Dictionary<string, List<SpeedUpSourceInfo>> Sources
	{
		get
		{
			return sources;
		}
		set
		{
			sources = value;
		}
	}

	public RefDateTime BecomesOffline
	{
		get
		{
			return becomesOffline;
		}
		set
		{
			becomesOffline = value;
		}
	}
}
