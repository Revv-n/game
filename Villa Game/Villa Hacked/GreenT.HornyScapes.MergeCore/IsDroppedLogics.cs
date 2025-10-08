using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class IsDroppedLogics
{
	private float _chance = -1f;

	public bool IsDropped(float pointsChance)
	{
		if (pointsChance > 1f)
		{
			pointsChance /= 100f;
		}
		return BaseLogics(pointsChance);
	}

	private static bool BaseLogics(float pointsChance)
	{
		return pointsChance >= Random.value;
	}

	public void SetChance(float targetChance)
	{
		_chance = targetChance / 100f;
	}
}
