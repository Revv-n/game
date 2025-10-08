using GreenT.HornyScapes.MiniEvents;
using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

public class Chance
{
	private readonly GarantChance _garantChance;

	private readonly int[] _chances;

	public int GarantID => _garantChance?.ID ?? (-1);

	public Chance(int[] chances)
		: this(null, chances)
	{
	}

	public Chance(GarantChance garantChance = null, int[] chances = null)
	{
		_garantChance = garantChance;
		_chances = chances;
	}

	public bool IsChance(int step)
	{
		int num = Random.Range(1, 100);
		int value = GetValue(step);
		return num <= value;
	}

	public int MaxSteps()
	{
		return _garantChance.GetMax();
	}

	public int GetValue(int step)
	{
		if (_garantChance != null)
		{
			int max = _garantChance.GetMax();
			step = Mathf.Min(step, max);
			return _garantChance.GetChance(step);
		}
		if (_chances != null)
		{
			int num = _chances.Length;
			step = Mathf.Min(step, num - 1);
			return _chances[step];
		}
		return 0;
	}
}
