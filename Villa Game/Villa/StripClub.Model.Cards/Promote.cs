using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Data;
using UniRx;

namespace StripClub.Model.Cards;

public class Promote : IPromote, IDisposable
{
	protected const int INITIAL_LEVEL = 1;

	protected const int INITIAL_PROGRESS = 0;

	protected readonly ReactiveProperty<int> level;

	protected readonly ReactiveProperty<int> progress;

	private CurrencyType promoteCostType;

	public IReadOnlyReactiveProperty<int> Level => level.ToReadOnlyReactiveProperty();

	public IReadOnlyReactiveProperty<int> Progress => progress.ToReadOnlyReactiveProperty();

	public PromoteState State { get; private set; }

	public IDictionary<int, PromotePattern> PromoteSettings { get; }

	public IReactiveProperty<bool> IsNew { get; }

	public int Target { get; private set; }

	public Cost LevelUpCost { get; private set; }

	public Promote(IDictionary<int, PromotePattern> promoteSettings, CurrencyType currencyType)
	{
		level = new ReactiveProperty<int>(1);
		IsNew = new ReactiveProperty<bool>(initialValue: true);
		State = PromoteState.Default;
		PromoteSettings = promoteSettings;
		progress = new ReactiveProperty<int>(0);
		PromotePattern value = PromoteSettings.First((KeyValuePair<int, PromotePattern> _settings) => _settings.Key == level.Value).Value;
		Target = value.promote_cards_value;
		LevelUpCost = new Cost(value.promote_resource_value, currencyType);
		promoteCostType = currencyType;
	}

	public void AddSoul(int value)
	{
		if (State != PromoteState.Maxed)
		{
			int promote_cards_value = PromoteSettings[level.Value].promote_cards_value;
			if (progress.Value + value >= promote_cards_value)
			{
				State = PromoteState.Promote;
			}
			progress.Value += value;
		}
	}

	public void LevelUp()
	{
		if (State == PromoteState.Promote)
		{
			progress.Value -= Target;
			UpdateValues(level.Value + 1, progress.Value);
			ReactiveProperty<int> reactiveProperty = level;
			int value = reactiveProperty.Value + 1;
			reactiveProperty.Value = value;
		}
	}

	public void Init(int level, int progress)
	{
		this.level.Value = level;
		this.progress.Value = progress;
		UpdateValues(level, progress);
	}

	public void UpdateValues(int level, int progress)
	{
		PromotePattern value = PromoteSettings.First((KeyValuePair<int, PromotePattern> _settings) => _settings.Key == level).Value;
		Target = value.promote_cards_value;
		LevelUpCost = new Cost(value.promote_resource_value, promoteCostType);
		if (!PromoteSettings.ContainsKey(this.level.Value + 1))
		{
			State = PromoteState.Maxed;
			return;
		}
		int promote_cards_value = PromoteSettings[level].promote_cards_value;
		State = ((progress >= promote_cards_value) ? PromoteState.Promote : PromoteState.Default);
	}

	public virtual void Dispose()
	{
		level.Dispose();
		progress.Dispose();
		(IsNew as IDisposable).Dispose();
	}
}
