using System;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model.Shop;
using StripClub.NewEvent.Data;
using UniRx;

namespace StripClub.Model;

[MementoHolder]
public class SpendEventEnergyTracker : ISavableState, IDisposable
{
	public struct Data
	{
		public readonly int Count;

		public readonly string Type;

		public Data(int spendCount, string lastType)
		{
			Count = spendCount;
			Type = lastType;
		}
	}

	[Serializable]
	public class SpendEventEnergyMemento : Memento
	{
		public int UseCount;

		public string LastType;

		public SpendEventEnergyMemento(SpendEventEnergyTracker lot)
			: base(lot)
		{
			UseCount = lot._spendCount;
			LastType = lot._lastType;
		}
	}

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly ISaver _saver;

	private readonly EventProvider _eventProvider;

	private int _spendCount;

	private string _lastType;

	private readonly Subject<Unit> _onReset = new Subject<Unit>();

	private readonly Subject<Data> _onUpdate = new Subject<Data>();

	private readonly CompositeDisposable _trackStream = new CompositeDisposable();

	public IObservable<Unit> OnReset => _onReset;

	public IObservable<Data> OnUpdate => _onUpdate;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SpendEventEnergyTracker(ICurrencyProcessor currencyProcessor, ISaver saver, EventProvider eventProvider)
	{
		_currencyProcessor = currencyProcessor;
		_saver = saver;
		_eventProvider = eventProvider;
	}

	public void Initialize()
	{
		_saver.Add(this);
		CreatChengEventStream();
		CreatSpendEventEnergyStream();
	}

	private void CreatSpendEventEnergyStream()
	{
		_currencyProcessor.GetSpendObservable(CurrencyType.EventEnergy)?.Subscribe(AddValue).AddTo(_trackStream);
	}

	private void CreatChengEventStream()
	{
		(from bundleType in _eventProvider.CurrentCalendarProperty.Select(GetEventType)
			where !string.IsNullOrEmpty(bundleType) && !bundleType.Equals(_lastType)
			select bundleType).Subscribe(ResetData).AddTo(_trackStream);
	}

	private string GetEventType((CalendarModel calendar, Event @event) tuple)
	{
		if (tuple.calendar == null || tuple.@event == null)
		{
			return "";
		}
		return tuple.@event.Bundle.Type;
	}

	private void AddValue(int value)
	{
		_spendCount += value;
		SendUpdate();
	}

	private void SendUpdate()
	{
		_onUpdate.OnNext(new Data(_spendCount, _lastType));
	}

	public void ResetData(string bundleType)
	{
		_onReset.OnNext(Unit.Default);
		_spendCount = 0;
		_lastType = bundleType;
		SendUpdate();
	}

	public string UniqueKey()
	{
		return "spend.event.energy.tracker";
	}

	public Memento SaveState()
	{
		return new SpendEventEnergyMemento(this);
	}

	public void LoadState(Memento memento)
	{
		SpendEventEnergyMemento spendEventEnergyMemento = (SpendEventEnergyMemento)memento;
		_spendCount = spendEventEnergyMemento.UseCount;
		_lastType = spendEventEnergyMemento.LastType;
		SendUpdate();
	}

	public void Dispose()
	{
		_trackStream.Dispose();
	}
}
