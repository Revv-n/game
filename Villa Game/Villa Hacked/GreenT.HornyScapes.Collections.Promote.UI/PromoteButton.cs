using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Collections.Promote.UI;

public class PromoteButton : MonoBehaviour
{
	[SerializeField]
	protected Button promoteButton;

	[SerializeField]
	private Image priceIcon;

	[SerializeField]
	private TextMeshProUGUI priceValue;

	private GameSettings gameSettings;

	private ICurrencyProcessor currencyProcessor;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	private PromoteNotifier _promoteNotifier;

	private CompositeDisposable disposables = new CompositeDisposable();

	public IPromote Source { get; private set; }

	public ICard Card { get; private set; }

	[Inject]
	public void Init(GameSettings gameSettings, ICurrencyProcessor currencyProcessor, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, PromoteNotifier promoteNotifier)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.gameSettings = gameSettings;
		this.currencyProcessor = currencyProcessor;
		_promoteNotifier = promoteNotifier;
	}

	protected virtual void Promote()
	{
		Cost levelUpCost = Source.LevelUpCost;
		if (!currencyProcessor.TrySpent(levelUpCost))
		{
			return;
		}
		Source.LevelUp();
		_promoteNotifier.Notify(levelUpCost);
		try
		{
			currencyAmplitudeAnalytic.SendSpentEvent(levelUpCost.Currency, levelUpCost.Amount, CurrencyAmplitudeAnalytic.SourceType.Promote, Card.ContentType);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	public void Set(IPromote promote, ICard card)
	{
		promoteButton.onClick.RemoveListener(Promote);
		Card = card;
		Source = promote;
		disposables.Clear();
		DisposableExtensions.AddTo<ReactiveCommand>(ReactiveCommandExtensions.ToReactiveCommand(Observable.Select<int, bool>(Observable.CombineLatest<int, int, int>((IObservable<int>)promote.Progress, (IObservable<int>)currencyProcessor.GetCountReactiveProperty(CurrencyType.Soft), (Func<int, int, int>)((int x, int y) => x)), (Func<int, bool>)((int _) => promote.State == PromoteState.Promote && currencyProcessor.IsEnough(Source.LevelUpCost))), false), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>((IObservable<int>)promote.Level, (Action<int>)delegate
		{
			priceValue.text = promote.LevelUpCost.Amount.ToString();
			priceIcon.sprite = gameSettings.CurrencySettings[promote.LevelUpCost.Currency, default(CompositeIdentificator)].Sprite;
		}), (ICollection<IDisposable>)disposables);
		promoteButton.onClick.AddListener(Promote);
	}

	private void OnDestroy()
	{
		promoteButton.onClick.RemoveAllListeners();
	}
}
