using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsEventManager
{
	public struct EventInfo
	{
		public readonly CurrencyType CurrencyType;

		public CompositeIdentificator ID;

		public EventInfo(CurrencyType currencyType, CompositeIdentificator id)
		{
			CurrencyType = currencyType;
			ID = id;
		}
	}

	private readonly Subject<EventInfo> _onEventEnd = new Subject<EventInfo>();

	private readonly Dictionary<EventStructureType, CurrencyType[]> _currencyTypesByEvent = new Dictionary<EventStructureType, CurrencyType[]>
	{
		{
			EventStructureType.Event,
			new CurrencyType[3]
			{
				CurrencyType.Event,
				CurrencyType.EventEnergy,
				CurrencyType.EventXP
			}
		},
		{
			EventStructureType.BattlePass,
			new CurrencyType[1] { CurrencyType.BP }
		},
		{
			EventStructureType.Mini,
			new CurrencyType[1] { CurrencyType.MiniEvent }
		}
	};

	public IObservable<EventInfo> OnEventEnd => _onEventEnd;

	public void TryRemoveEventPoints(EventStructureType eventType, CompositeIdentificator id = default(CompositeIdentificator))
	{
		if (_currencyTypesByEvent.TryGetValue(eventType, out var value))
		{
			CurrencyType[] array = value;
			foreach (CurrencyType currencyType in array)
			{
				_onEventEnd.OnNext(new EventInfo(currencyType, id));
			}
		}
	}
}
