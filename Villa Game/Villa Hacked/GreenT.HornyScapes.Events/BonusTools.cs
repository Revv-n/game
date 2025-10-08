using System;
using GreenT.Bonus;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using Merge;

namespace GreenT.HornyScapes.Events;

public static class BonusTools
{
	public static string ToString(this IBonus bonus, int level)
	{
		BonusType bonusType = bonus.BonusType;
		double[] values = bonus.Values;
		if (!IsPercentBonusType(bonusType) && !IsCountBonusType(bonusType))
		{
			return $"+{values[level]}";
		}
		return $"{values[level]}%";
	}

	public static bool IsPercentBonusType(BonusType bonusType)
	{
		return bonusType == BonusType.decreaseReloadTime;
	}

	public static bool IsCountBonusType(BonusType bonusType)
	{
		return bonusType == BonusType.increaseProductionValue;
	}

	public static GIKey GetSpriteMaxOpenedSpawner(GameItemConfigManager gameItemConfigManager, CharacterMultiplierBonus bonus)
	{
		return GetMaxOpenedGIKey(GetGameItemKey(gameItemConfigManager, bonus));
	}

	private static GIKey GetGameItemKey(GameItemConfigManager gameItemConfigManager, CharacterMultiplierBonus bonus)
	{
		if (gameItemConfigManager.TryGetConfig(bonus.AffectedSpawnerId[0], out var giConfig))
		{
			return giConfig.Key;
		}
		throw new Exception().SendException("Can't find item by UniqID: " + bonus.AffectedSpawnerId[0]);
	}

	private static GIKey GetMaxOpenedGIKey(GIKey key)
	{
		return Controller<CollectionController>.Instance.GetMaxOpened(key);
	}
}
