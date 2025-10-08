using System;
using GreenT.Bonus;
using GreenT.Data;
using GreenT.Multiplier;
using JetBrains.Annotations;
using StripClub.Model;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Booster.Effect;

[Serializable]
[MementoHolder]
public class BoosterIncrementBonus : ISavableState, IBonus<int>, IValuableBonus<int>, ISimpleBonus, ICommand, ITypeBonus
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public TimeSpan TimeLeft { get; }

		public DateTime LastApplied { get; }

		public Memento(BoosterIncrementBonus bonus)
			: base(bonus)
		{
			TimeLeft = bonus.ApplyTimer.TimeLeft;
			LastApplied = bonus.LastApplied;
		}
	}

	public readonly Subject<Unit> OnApplied = new Subject<Unit>();

	[CanBeNull]
	public readonly string SummonType;

	[CanBeNull]
	public readonly int[] SummonTabID;

	private readonly IMultiplierTotalContainer<int> _multipliersContainer;

	public int BoosterID { get; }

	public int Values { get; }

	public string Name { get; }

	public BonusType BonusType { get; }

	public IMultiplier Multiplier { get; }

	public bool IsApplied { get; private set; }

	public GenericTimer ApplyTimer { get; }

	public DateTime LastApplied { get; set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public BoosterIncrementBonus(IMultiplierTotalContainer<int> multipliersContainer, int boosterID, string name, int value, BonusType bonusType, [CanBeNull] string summonType, [CanBeNull] int[] summonTabID)
	{
		Name = name;
		Values = value;
		BonusType = bonusType;
		SummonType = summonType;
		SummonTabID = summonTabID;
		ApplyTimer = new GenericTimer();
		IMultiplier multiplier2;
		if (bonusType != BonusType.FreeSummon)
		{
			IMultiplier multiplier = new BoosterSummingMultiplier(value);
			multiplier2 = multiplier;
		}
		else
		{
			IMultiplier multiplier = new MinCompositeMultiplier(value);
			multiplier2 = multiplier;
		}
		Multiplier = multiplier2;
		_multipliersContainer = multipliersContainer;
		BoosterID = boosterID;
	}

	public void StartTimer()
	{
		ApplyTimer.Start(TimeSpan.FromSeconds(Values));
	}

	public void Apply()
	{
		_multipliersContainer.AddToTotal(Multiplier);
		StartTimer();
		IsApplied = true;
	}

	public void Undo()
	{
		_multipliersContainer.RemoveFromTotal(Multiplier);
		ApplyTimer.Stop();
		IsApplied = false;
	}

	public string UniqueKey()
	{
		return $"{Name}.{BoosterID}";
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento memento2)
		{
			LastApplied = memento2.LastApplied;
			ApplyTimer.Start(memento2.TimeLeft);
		}
	}
}
