using System;
using System.Collections.Generic;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.Types;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.NewEvent.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class WalletProvider : IInitializable, IDisposable
{
	private readonly BattlePassProvider _battlePassProvider;

	private readonly IPlayerBasics _playerBasics;

	private readonly Dictionary<CurrencyType, ICurrenciesActionContainer> containers = new Dictionary<CurrencyType, ICurrenciesActionContainer>();

	private readonly EventDataCurrencyProvider _eventDataCurrencyProvider;

	private readonly BattlePassCurrencyProvider _battlePassCurrencyProvider;

	private readonly MiniEventCurrencyProvider _miniEventCurrencyProvider;

	private readonly RelationshipsCurrencyProvider _relationshipsCurrencyProvider;

	private Currencies MainBalance => _playerBasics.Balance;

	private RestorableValue<int> Energy => _playerBasics.Energy;

	private RestorableEventEnergyValue<int> EventEnergy => _playerBasics.EventEnergy;

	public WalletProvider(IPlayerBasics playerBasics, EventProvider eventProvider, BattlePassProvider battlePassProvider, TrackableCurrencyActionContainerTracker trackableCurrencyActionContainerTracker)
	{
		_playerBasics = playerBasics;
		_battlePassProvider = battlePassProvider;
		_eventDataCurrencyProvider = new EventDataCurrencyProvider(eventProvider);
		_battlePassCurrencyProvider = new BattlePassCurrencyProvider(battlePassProvider, _playerBasics.Balance);
		_miniEventCurrencyProvider = new MiniEventCurrencyProvider(MainBalance, trackableCurrencyActionContainerTracker);
		_relationshipsCurrencyProvider = new RelationshipsCurrencyProvider(MainBalance, trackableCurrencyActionContainerTracker);
	}

	public void Initialize()
	{
		_eventDataCurrencyProvider.Initialize();
		_battlePassCurrencyProvider.Initialize();
	}

	public bool TryGetContainer(CurrencyType type, out ICurrenciesActionContainer container, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		container = null;
		if (IsMessage(type))
		{
			return TryGetMessageContainer(out container);
		}
		if (IsEventType(type))
		{
			return TryGetEventContainer(type, out container);
		}
		if (IsBattlePass(type))
		{
			return TryGetBattlePassContainer(out container);
		}
		if (IsReactiveType(type))
		{
			return TryGetReactiveContainer(type, out container);
		}
		if (IsEventEnergy(type))
		{
			return TryGetEventEnergyContainer(out container, type);
		}
		if (IsEnergy(type))
		{
			return TryGetEnergyContainer(out container, type);
		}
		if (IsMiniEventType(type))
		{
			return TryGetMiniEventContainer(compositeIdentificator, out container);
		}
		if (IsLovePointsType(type))
		{
			return TryGetLovePointsContainer(compositeIdentificator, out container);
		}
		if (IsPresentType(type))
		{
			return TryGetPresentContainer(type, out container);
		}
		return false;
	}

	private bool GetContainer(CurrencyType currencyType, out ICurrenciesActionContainer container, Func<ICurrenciesActionContainer> func)
	{
		if (!containers.ContainsKey(currencyType))
		{
			containers.Add(currencyType, func());
		}
		container = containers[currencyType];
		return container != null;
	}

	private bool IsReactiveType(CurrencyType type)
	{
		if (type != 0 && type != CurrencyType.Hard && type != CurrencyType.Star && type != CurrencyType.Jewel)
		{
			return type == CurrencyType.Contracts;
		}
		return true;
	}

	private bool IsMessage(CurrencyType type)
	{
		return type == CurrencyType.Message;
	}

	private static bool IsEnergy(CurrencyType type)
	{
		if (type != CurrencyType.Energy)
		{
			return type == CurrencyType.EventEnergy;
		}
		return true;
	}

	private static bool IsEventEnergy(CurrencyType type)
	{
		return type == CurrencyType.EventEnergy;
	}

	private bool IsBattlePass(CurrencyType type)
	{
		return type == CurrencyType.BP;
	}

	private bool IsEventType(CurrencyType type)
	{
		if (type != CurrencyType.Event)
		{
			return type == CurrencyType.EventXP;
		}
		return true;
	}

	private bool IsMiniEventType(CurrencyType type)
	{
		return type == CurrencyType.MiniEvent;
	}

	private bool IsLovePointsType(CurrencyType type)
	{
		return type == CurrencyType.LovePoints;
	}

	private bool IsPresentType(CurrencyType type)
	{
		if (type != CurrencyType.Present1 && type != CurrencyType.Present2 && type != CurrencyType.Present3)
		{
			return type == CurrencyType.Present4;
		}
		return true;
	}

	private bool TryGetEnergyContainer(out ICurrenciesActionContainer container, CurrencyType currencyType)
	{
		return GetContainer(currencyType, out container, () => new EnergyCurrenciesActionContainer(Energy, MainBalance));
	}

	private bool TryGetEventEnergyContainer(out ICurrenciesActionContainer container, CurrencyType currencyType)
	{
		return GetContainer(currencyType, out container, () => new EventEnergyCurrenciesActionContainer(EventEnergy, MainBalance));
	}

	private bool TryGetReactiveContainer(CurrencyType currencyType, out ICurrenciesActionContainer container)
	{
		return GetContainer(currencyType, out container, () => new ReactiveCurrenciesActionContainer(currencyType, MainBalance));
	}

	private bool TryGetEventContainer(CurrencyType currencyType, out ICurrenciesActionContainer container)
	{
		return _eventDataCurrencyProvider.TryGetContainer(currencyType, out container);
	}

	private bool TryGetBattlePassContainer(out ICurrenciesActionContainer container)
	{
		return _battlePassCurrencyProvider.TryGetContainer(out container);
	}

	private bool TryGetMiniEventContainer(CompositeIdentificator compositeIdentificator, out ICurrenciesActionContainer container)
	{
		return _miniEventCurrencyProvider.TryGetContainer(compositeIdentificator, out container);
	}

	private bool TryGetLovePointsContainer(CompositeIdentificator compositeIdentificator, out ICurrenciesActionContainer container)
	{
		return _relationshipsCurrencyProvider.TryGetContainer(compositeIdentificator, out container);
	}

	private bool TryGetPresentContainer(CurrencyType currencyType, out ICurrenciesActionContainer container)
	{
		return GetContainer(currencyType, out container, () => new ReactiveCurrenciesActionContainer(currencyType, MainBalance));
	}

	private bool TryGetMessageContainer(out ICurrenciesActionContainer container)
	{
		Debug.LogError(new Exception("This system is not suitable for using the message. Consult a specialist (У Бояна спросите)"));
		container = null;
		return false;
	}

	public void Dispose()
	{
		_eventDataCurrencyProvider.Dispose();
		foreach (ICurrenciesActionContainer value in containers.Values)
		{
			value.Dispose();
		}
	}
}
