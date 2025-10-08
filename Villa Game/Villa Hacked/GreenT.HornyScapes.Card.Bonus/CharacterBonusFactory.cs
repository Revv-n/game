using System;
using GreenT.Bonus;
using GreenT.HornyScapes.GameItems;
using GreenT.Multiplier;
using Zenject;

namespace GreenT.HornyScapes.Card.Bonus;

public class CharacterBonusFactory : IFactory<CharacterBonusParameters, ISimpleBonus>, IFactory
{
	private readonly MultiplierManager _multiplierManager;

	private readonly GameItemConfigManager _gameItemConfigManager;

	public CharacterBonusFactory(MultiplierManager multiplierManager, GameItemConfigManager gameItemConfigManager)
	{
		_multiplierManager = multiplierManager;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public ISimpleBonus Create(CharacterBonusParameters parameters)
	{
		try
		{
			string spawnerName = GetSpawnerName(parameters.AffectAll, parameters.AffectedSpawnerID);
			return new CharacterMultiplierBonus(_multiplierManager.GetCollection(parameters.BonusType), parameters.GetValue<double[]>(), parameters.NameKey, parameters.BonusType, parameters.AffectAll, parameters.AffectedSpawnerID, spawnerName);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can not create MultiplierBonus of type: " + parameters.BonusType);
		}
	}

	private string GetSpawnerName(bool affectAll, int[] affectedSpawnerId)
	{
		if (affectAll)
		{
			return "item.AllSpawners";
		}
		if (_gameItemConfigManager.TryGetConfig(affectedSpawnerId[0], out var giConfig))
		{
			return "item." + giConfig.Key.ToString();
		}
		throw new Exception().SendException("Can't find item by UniqID: " + affectedSpawnerId[0]);
	}
}
