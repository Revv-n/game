using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Bank;

public class ResetAfterBundleLotController
{
	private readonly Dictionary<string, HashSet<BundleLot>> _bundleLots = new Dictionary<string, HashSet<BundleLot>>();

	private static readonly Dictionary<EventStructureType, string> Keys = new Dictionary<EventStructureType, string>
	{
		{
			EventStructureType.Mini,
			"minievent"
		},
		{
			EventStructureType.BattlePass,
			"battlepass"
		},
		{
			EventStructureType.Event,
			"event"
		},
		{
			EventStructureType.Sellout,
			"sellout"
		}
	};

	public void Add(string[] keys, BundleLot bundleLot)
	{
		if (keys == null || (keys != null && keys.Length == 0) || bundleLot == null)
		{
			return;
		}
		foreach (string key in keys)
		{
			if (!_bundleLots.ContainsKey(key))
			{
				_bundleLots.Add(key, new HashSet<BundleLot>());
			}
			_bundleLots[key].Add(bundleLot);
		}
	}

	public void TryClear(EventStructureType eventType, int id)
	{
		string key = $"{Keys[eventType]}:{id}";
		if (!_bundleLots.TryGetValue(key, out var value))
		{
			return;
		}
		foreach (BundleLot item in value)
		{
			item.ForceReset();
		}
	}
}
