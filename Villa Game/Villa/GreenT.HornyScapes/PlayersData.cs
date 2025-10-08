using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Types;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes;

[Serializable]
[MementoHolder]
public class PlayersData : IPlayerBasics, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int level;

		public bool noNeedMigrate93To94;

		public Currencies balance;

		public Memento(PlayersData playersData)
			: base(playersData)
		{
			noNeedMigrate93To94 = playersData.noNeedMigrate93To94 || balance == null;
			level = playersData.Level.Value;
		}
	}

	private readonly PlayerEnergyFactory energyFactory;

	private readonly PlayerEventEnergyFactory eventEnergyFactory;

	public bool noNeedMigrate93To94;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Currencies Balance { get; private set; }

	public ReactiveProperty<int> Level { get; private set; }

	public RestorableValue<int> Energy { get; private set; }

	public RestorableEventEnergyValue<int> EventEnergy { get; private set; }

	string ISavableState.UniqueKey()
	{
		return "PlayersData";
	}

	public PlayersData(Currencies balance, PlayerEnergyFactory energyFactory, PlayerEventEnergyFactory eventEnergyFactory)
	{
		this.energyFactory = energyFactory;
		this.eventEnergyFactory = eventEnergyFactory;
		Balance = balance;
		Level = new ReactiveProperty<int>();
	}

	public void Init()
	{
		if (Energy != null)
		{
			Energy.OnValueChanged -= OnEnergyValueChanged;
		}
		if (EventEnergy != null)
		{
			EventEnergy.OnValueChanged -= OnEventEnergyValueChanged;
		}
		Energy = TryGetEnergy(OnEnergyValueChanged, energyFactory);
		EventEnergy = TryGetEventEnergy(OnEventEnergyValueChanged, eventEnergyFactory);
		void OnEnergyValueChanged(int value)
		{
			Balance.Get(CurrencyType.Energy).Value = value;
		}
		void OnEventEnergyValueChanged(int value)
		{
			Balance.Get(CurrencyType.EventEnergy).Value = value;
		}
	}

	private RestorableValue<int> TryGetEnergy(Action<int> onValueChanged, BaseEnergyFactory baseEnergyFactory)
	{
		RestorableValue<int> restorableValue = baseEnergyFactory.Create();
		onValueChanged(restorableValue.Value);
		restorableValue.OnValueChanged += onValueChanged;
		return restorableValue;
	}

	private RestorableEventEnergyValue<int> TryGetEventEnergy(Action<int> onValueChanged, BaseEnergyFactory baseEnergyFactory)
	{
		RestorableEventEnergyValue<int> restorableEventEnergyValue = baseEnergyFactory.CreateEventEnergy();
		onValueChanged(restorableEventEnergyValue.Value);
		restorableEventEnergyValue.OnValueChanged += onValueChanged;
		return restorableEventEnergyValue;
	}

	GreenT.Data.Memento ISavableState.SaveState()
	{
		return new Memento(this);
	}

	void ISavableState.LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		Migrate93To94(memento2);
		Level.Value = memento2.level;
	}

	public void AddCurrencyType(CurrencyType type, SimpleCurrency currency, CompositeIdentificator compositeIdentificator)
	{
		Balance.Set(type, currency, compositeIdentificator);
	}

	private void Migrate93To94(Memento memento)
	{
		noNeedMigrate93To94 = memento.noNeedMigrate93To94;
		if (noNeedMigrate93To94 || memento.balance == null)
		{
			return;
		}
		foreach (System.Collections.Generic.KeyValuePair<SimpleCurrency.CurrencyKey, SimpleCurrency> item in memento.balance.CurrenciesDict)
		{
			if (Balance.CurrenciesDict.Keys.Contains(item.Key))
			{
				Balance.Get(item.Key.CurrencyType).Value = item.Value.Count.Value;
			}
		}
		noNeedMigrate93To94 = true;
	}
}
