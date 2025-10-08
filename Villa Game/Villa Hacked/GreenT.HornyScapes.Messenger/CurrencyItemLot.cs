using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger;

public class CurrencyItemLot : IItemLot, IDisposable
{
	private readonly CurrencyType _currencyType;

	private readonly CompositeIdentificator _identificator;

	private readonly ICurrencyProcessor _currencyProcessor;

	private IDisposable _updateAlternativeSpriteStream;

	public int TargetCount { get; }

	public Sprite Icon { get; private set; }

	public CurrencyItemLot(CurrencyType currencyType, CompositeIdentificator identificator, int count, Sprite icon, ICurrencyProcessor currencyProcessor)
	{
		_currencyType = currencyType;
		_identificator = identificator;
		_currencyProcessor = currencyProcessor;
		TargetCount = count;
		Icon = icon;
	}

	public void Initialization(IObservable<Sprite> updateAlternativeSprite)
	{
		_updateAlternativeSpriteStream = ObservableExtensions.Subscribe<Sprite>(updateAlternativeSprite, (Action<Sprite>)delegate(Sprite icon)
		{
			Icon = icon;
		});
	}

	public int GetCurrentCount()
	{
		return GetPlayersItemCount();
	}

	public bool CheckIsEnough()
	{
		return GetCurrentCount() >= TargetCount;
	}

	public void Buy()
	{
		_currencyProcessor.TrySpent(_currencyType, TargetCount, _identificator);
	}

	private int GetPlayersItemCount()
	{
		return Mathf.Min(_currencyProcessor.GetCount(_currencyType, _identificator), TargetCount);
	}

	public void Dispose()
	{
		_updateAlternativeSpriteStream.Dispose();
	}
}
