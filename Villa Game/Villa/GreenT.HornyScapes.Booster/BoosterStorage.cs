using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Bonus;
using GreenT.Data;
using GreenT.HornyScapes.Booster.Effect;
using GreenT.Model.Collections;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Booster;

[MementoHolder]
public class BoosterStorage : Storage<BoosterModel>, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public DateTime SaveTime { get; }

		public List<BoosterModel.Memento> Boosters { get; }

		public Memento(BoosterStorage storage)
			: base(storage)
		{
			SaveTime = storage.GetTime();
			Boosters = storage.collection.Select((BoosterModel model) => model.SaveState()).ToList();
		}
	}

	public readonly ReactiveProperty<int> BoosterToLoad = new ReactiveProperty<int>();

	private readonly BonusFactory _bonusFactory;

	private readonly BoosterModelFactory _modelFactory;

	private readonly IClock _clock;

	private readonly BoosterMapperManager _mapperManager;

	private readonly TimeInstaller.TimerCollection _timerCollection;

	private int _sequenceCounter;

	public SavableStatePriority Priority => SavableStatePriority.BoosterStorage;

	public BoosterStorage(BoosterModelFactory modelFactory, IClock clock, [InjectOptional] TimeInstaller.TimerCollection timerCollection, BoosterMapperManager mapperManager, BonusFactory bonusFactory)
	{
		_modelFactory = modelFactory;
		_clock = clock;
		_timerCollection = timerCollection;
		_mapperManager = mapperManager;
		_bonusFactory = bonusFactory;
	}

	public void Setup(BoosterMapper mapper)
	{
		BoosterModel boosterModel = collection.FirstOrDefault((BoosterModel item) => item.ID == mapper.booster_id);
		if (boosterModel != null)
		{
			boosterModel.Prolong(mapper.booster_time);
			return;
		}
		int nextID = GetNextID();
		Create(nextID, mapper, 0L);
	}

	private void Create(int sequenceID, BoosterMapper mapper, long time = 0L)
	{
		BoosterModel boosterModel = _modelFactory.Create(sequenceID, mapper);
		ISimpleBonus bonus = CreateBonus(mapper);
		if (time == 0L)
		{
			boosterModel.Activate(bonus, 0L);
		}
		else
		{
			boosterModel.Activate(bonus, time);
		}
		_timerCollection.Add(boosterModel.Timer);
		Add(boosterModel);
	}

	private ISimpleBonus CreateBonus(BoosterMapper mapper)
	{
		BoosterBonusParameters parameters = new BoosterBonusParameters(mapper.booster_id, mapper.bonus_type.ToString(), mapper.bonus_type, mapper.bonus_value, mapper.summon_type, mapper.tab_id);
		return _bonusFactory.Create(parameters);
	}

	private int GetNextID()
	{
		int sequenceCounter = _sequenceCounter;
		_sequenceCounter++;
		return sequenceCounter;
	}

	private DateTime GetTime()
	{
		return _clock.GetTime();
	}

	public string UniqueKey()
	{
		return "booster_storage";
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		foreach (BoosterModel.Memento boosterMemento in memento2.Boosters)
		{
			BoosterMapper mapper = _mapperManager.Collection.First((BoosterMapper item) => item.booster_id == boosterMemento.ID);
			TimeSpan timeLeft = boosterMemento.TimeLeft;
			TimeSpan timeSpan = _clock.GetTime() - memento2.SaveTime;
			if (timeSpan.Ticks > 0)
			{
				if (memento2.SaveTime != default(DateTime))
				{
					timeLeft -= timeSpan;
				}
				if (timeLeft.Ticks > 0)
				{
					Create(boosterMemento.SequenceID, mapper, (int)timeLeft.TotalSeconds);
				}
			}
		}
		BoosterToLoad.Value = (from model in collection
			select model.Bonus as ITypeBonus into bonus
			where bonus != null
			select bonus).Count(delegate(ITypeBonus bonus)
		{
			BonusType bonusType = bonus.BonusType;
			return bonusType == BonusType.IncreaseBaseEnergy || bonusType == BonusType.IncreaseEnergyRechargeSpeed;
		});
		if (collection.Any())
		{
			_sequenceCounter = collection.IndexOf(collection.Last());
		}
	}
}
