using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes;

[Serializable]
public class ResetDailyPriceLogics : IResetDailyPriceLogics
{
	[Serializable]
	public class Memento
	{
		public bool IsFree;

		public int PurchaseCount;

		public DateTime LastBuy;

		public Memento()
		{
			IsFree = true;
			PurchaseCount = 0;
			LastBuy = DateTime.MinValue;
		}

		public Memento(ResetDailyPriceLogics logics)
		{
			IsFree = logics.IsFree;
			LastBuy = logics.LastReceivedChangeDate;
			PurchaseCount = logics.PurchaseCount;
		}
	}

	private readonly int _basePrice;

	private readonly int _priceStep;

	private readonly int _maxPrice;

	private readonly IClock _clock;

	private GenericTimer _timer;

	private readonly CompositeDisposable _timerStream = new CompositeDisposable();

	private readonly Subject<Price<int>> _onPriceUpdate = new Subject<Price<int>>();

	public bool IsFree { get; private set; }

	public int PurchaseCount { get; private set; }

	public DateTime LastReceivedChangeDate { get; private set; }

	public IObservable<Price<int>> OnPriceUpdate => _onPriceUpdate;

	public Price<int> Price
	{
		get
		{
			if (!IsFree)
			{
				return GetTargetPrice();
			}
			return Price<int>.Free;
		}
	}

	public ResetDailyPriceLogics(int basePrice, int priceStep, int maxPrice, IClock clock, bool isFree)
	{
		_basePrice = basePrice;
		_priceStep = priceStep;
		_maxPrice = maxPrice;
		_clock = clock;
		IsFree = isFree;
		PurchaseCount = 0;
		LastReceivedChangeDate = _clock.GetDate();
		ResetTimer();
	}

	public void ForceDailyInfo()
	{
		ResetDailyInfo();
	}

	public void OnPurchase()
	{
		if (!IsFree)
		{
			PurchaseCount++;
		}
		else
		{
			IsFree = false;
		}
		UpdateLastReceivedChangeDate();
		SendPriceUpdateEvent();
	}

	private Price<int> GetTargetPrice()
	{
		return new Price<int>(Math.Min(_basePrice + _priceStep * PurchaseCount, _maxPrice), CurrencyType.Hard, default(CompositeIdentificator));
	}

	private void UpdateLastReceivedChangeDate()
	{
		LastReceivedChangeDate = _clock.GetDate();
	}

	public void ResetTimer()
	{
		_timerStream.Clear();
		_timer?.Dispose();
		_timer = new GenericTimer(_clock.GetTimeEndDay());
		_timer.OnTimeIsUp.Subscribe(delegate
		{
			TryResetDailyInfo();
		}).AddTo(_timerStream);
	}

	private void TryResetDailyInfo()
	{
		DateTime date = LastReceivedChangeDate.Date;
		DateTime minValue = DateTime.MinValue;
		if (!(date == minValue.Date) && !(date >= _clock.GetDate()))
		{
			ResetDailyInfo();
		}
	}

	private void ResetDailyInfo()
	{
		ResetPurchaseCount();
		UpdateLastReceivedChangeDate();
		ResetTimer();
		SendPriceUpdateEvent();
	}

	private void SendPriceUpdateEvent()
	{
		_onPriceUpdate.OnNext(Price);
	}

	private void ResetPurchaseCount()
	{
		PurchaseCount = 0;
	}

	public void Load(Memento memento)
	{
		LastReceivedChangeDate = memento.LastBuy;
		PurchaseCount = memento.PurchaseCount;
		IsFree = memento.IsFree;
		TryResetDailyInfo();
		SendPriceUpdateEvent();
	}

	public Memento Save()
	{
		return new Memento(this);
	}
}
