using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.UI;
using GreenT.HornyScapes.UI.Promote;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using StripClub.UI.Collections.Promote;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Collections.Promote.UI;

public class CardPromotionInterfaceView : CharacterView
{
	[SerializeField]
	private BonusView bonusView;

	[SerializeField]
	private PromoteButton promoteButton;

	[SerializeField]
	private PriceWithFreeView priceView;

	[SerializeField]
	private LockedPromoteButton lockPromoteButton;

	private ICurrencyProcessor currencyProcessor;

	private CompositeDisposable disposable = new CompositeDisposable();

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
	}

	public override void Set(CharacterSettings card)
	{
		base.Set(card);
		CompositeDisposable obj = disposable;
		if (obj != null)
		{
			obj.Clear();
		}
		IPromote promote = card.Promote;
		if (promote != null)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Merge<int>(Observable.Skip<int>(Observable.Merge<int>(Observable.Skip<int>((IObservable<int>)promote.Progress, 1), new IObservable<int>[1] { (IObservable<int>)promote.Level }), 1), new IObservable<int>[1] { (IObservable<int>)currencyProcessor.GetCountReactiveProperty(CurrencyType.Soft) }), (Action<int>)delegate
			{
				SetChooseButtonView(promote);
			}), (ICollection<IDisposable>)disposable);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.TakeUntilDisable<int>((IObservable<int>)promote.Level, (Component)this), (Action<int>)delegate
			{
				bonusView.Init(card.Public.Bonus as CharacterMultiplierBonus);
			}), (ICollection<IDisposable>)disposable);
		}
	}

	private void SetChooseButtonView(IPromote promote)
	{
		bool flag = currencyProcessor.IsEnough(promote.LevelUpCost);
		bool flag2 = promote.State == PromoteState.Promote;
		if (flag && flag2)
		{
			promoteButton.Set(promote, base.Source.Public);
		}
		else
		{
			Price<int> price = new Price<int>(promote.LevelUpCost.Amount, promote.LevelUpCost.Currency, default(CompositeIdentificator));
			priceView.Set(price);
		}
		priceView.SetValueColor((!flag) ? 1 : 0);
		promoteButton.gameObject.SetActive(flag && flag2);
		priceView.gameObject.SetActive(!flag || !flag2);
		lockPromoteButton.Set(flag, flag2);
	}

	private void OnDestroy()
	{
		disposable.Dispose();
	}
}
