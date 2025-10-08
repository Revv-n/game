using System;
using GreenT.HornyScapes.BattlePassSpace;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class CurrencyAnalyticProcessingService : IInitializable, IDisposable
{
	private const int MergeCollectedCurrencyThreshold = 10;

	private IDisposable _calendarChangeStream;

	private BattlePass _currentBattlePass;

	private readonly BattlePassSettingsProvider _battlePassProvider;

	private readonly CurrencyAmplitudeAnalytic _currencyAnalytic;

	private readonly CalendarQueue _calendarQueue;

	public CurrencyAnalyticProcessingService(CurrencyAmplitudeAnalytic currencyAnalytic, CalendarQueue calendarQueue, BattlePassSettingsProvider battlePassProvider)
	{
		_currencyAnalytic = currencyAnalytic;
		_calendarQueue = calendarQueue;
		_battlePassProvider = battlePassProvider;
	}

	public void Initialize()
	{
		_calendarChangeStream = ObservableExtensions.Subscribe<CalendarModel>(_calendarQueue.OnCalendarActiveNotNull(), (Action<CalendarModel>)SetCurrentBattlePass);
	}

	public void Dispose()
	{
		_calendarChangeStream.Dispose();
	}

	public void SendCurrencyEvent(LinkedContent linkedContent)
	{
		if (linkedContent is CurrencyLinkedContent currencyLinkedContent)
		{
			if (IsBattlePassCurrency(currencyLinkedContent) && IsCurrencyRecieved(currencyLinkedContent.Quantity))
			{
				HandleBattlePassCurrencyEvent(currencyLinkedContent);
			}
			else
			{
				_currencyAnalytic.SendReceivedOneEvent(currencyLinkedContent);
			}
		}
	}

	private void SetCurrentBattlePass(CalendarModel calendarModel)
	{
		if (_battlePassProvider.TryGetBattlePass(calendarModel.BalanceId, out var battlePass))
		{
			_currentBattlePass = battlePass;
		}
	}

	private void HandleBattlePassCurrencyEvent(CurrencyLinkedContent linkedContent)
	{
		if (_currentBattlePass != null)
		{
			if (linkedContent.AnalyticData.SourceType == CurrencyAmplitudeAnalytic.SourceType.MergeCollect)
			{
				HandleMergeCollectedCurrency(linkedContent);
			}
			else
			{
				_currencyAnalytic.SendReceivedOneEvent(linkedContent);
			}
		}
	}

	private void HandleMergeCollectedCurrency(CurrencyLinkedContent linkedContent)
	{
		if (IsEnoughCurrencyCollected(linkedContent.Quantity))
		{
			_currencyAnalytic.SendReceivedEvent(linkedContent.Currency, 10, linkedContent.AnalyticData.SourceType, linkedContent.CompositeIdentificator);
			_currentBattlePass.Data.MergedCurrencyData.Reset();
		}
	}

	private bool IsEnoughCurrencyCollected(int quantity)
	{
		_currentBattlePass.Data.MergedCurrencyData.MergedCurrencyQuantity += quantity;
		return _currentBattlePass.Data.MergedCurrencyData.MergedCurrencyQuantity >= 10;
	}

	private bool IsBattlePassCurrency(CurrencyLinkedContent currencyContent)
	{
		return currencyContent.Currency == CurrencyType.BP;
	}

	private bool IsCurrencyRecieved(int quantity)
	{
		return quantity > 0;
	}
}
