using System;
using GreenT.Bonus;
using GreenT.Data;
using GreenT.Multiplier;
using Zenject;

namespace GreenT.HornyScapes.Booster.Effect;

public class BoosterBonusFactory : IFactory<BoosterBonusParameters, ISimpleBonus>, IFactory
{
	private readonly MultiplierManager _multiplierManager;

	private readonly TimeInstaller.TimerCollection _timerCollection;

	private readonly ISaver _saver;

	private BoosterBonusFactory(MultiplierManager multiplierManager, ISaver saver, [InjectOptional] TimeInstaller.TimerCollection timerCollection)
	{
		_saver = saver;
		_timerCollection = timerCollection;
		_multiplierManager = multiplierManager;
	}

	public ISimpleBonus Create(BoosterBonusParameters parameters)
	{
		try
		{
			BoosterIncrementBonus boosterIncrementBonus = new BoosterIncrementBonus(_multiplierManager.GetCollection(parameters.BonusType), parameters.UniqParentID, parameters.NameKey, parameters.GetValue<int>(), parameters.BonusType, parameters.SummonType, parameters.SummonTabID);
			if (boosterIncrementBonus.BonusType == BonusType.FreeSummon)
			{
				_saver.Add(boosterIncrementBonus);
			}
			_timerCollection.Add(boosterIncrementBonus.ApplyTimer);
			return boosterIncrementBonus;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Can not create {typeof(BoosterIncrementBonus)} of type: " + parameters.BonusType);
		}
	}
}
