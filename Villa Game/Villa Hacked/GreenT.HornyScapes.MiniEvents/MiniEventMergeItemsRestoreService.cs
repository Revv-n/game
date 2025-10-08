using System;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventMergeItemsRestoreService : MiniEventMergeItemsService, IInitializable, IDisposable
{
	private readonly Currencies _currencies;

	private IDisposable _currencyCreationStream;

	public MiniEventMergeItemsRestoreService(MiniEventMergeItemsDataCase mergeItemsDataCase, MiniEventInventoryItemsDataCase inventoryItemsDataCase, MiniEventPocketItemsDataCase pocketItemsDataCase, Currencies currencies)
		: base(mergeItemsDataCase, inventoryItemsDataCase, pocketItemsDataCase)
	{
		_currencies = currencies;
	}

	public void Initialize()
	{
		_currencyCreationStream = ObservableExtensions.Subscribe<(CurrencyType, SimpleCurrency)>(Observable.Where<(CurrencyType, SimpleCurrency)>(_currencies.OnNewCurrency, (Func<(CurrencyType, SimpleCurrency), bool>)(((CurrencyType, SimpleCurrency) currency) => currency.Item1 == CurrencyType.MiniEvent)), (Action<(CurrencyType, SimpleCurrency)>)delegate((CurrencyType, SimpleCurrency) currency)
		{
			RestoreCurrencyOnCreate(currency.Item2);
		});
	}

	private void RestoreCurrencyOnCreate(SimpleCurrency currency)
	{
		_mergeItemsDataCase.LoadItems(currency.Identificator);
		_inventoryItemsDataCase.LoadItems(currency.Identificator);
		_pocketItemsDataCase.LoadItems(currency.Identificator);
	}

	public void Dispose()
	{
		_currencyCreationStream.Dispose();
	}
}
