using System;
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
		(from _ in promote.Progress.CombineLatest(currencyProcessor.GetCountReactiveProperty(CurrencyType.Soft), (int x, int y) => x)
			select promote.State == PromoteState.Promote && currencyProcessor.IsEnough(Source.LevelUpCost)).ToReactiveCommand(initialValue: false).AddTo(disposables);
		promote.Level.Subscribe(delegate
		{
			priceValue.text = promote.LevelUpCost.Amount.ToString();
			priceIcon.sprite = gameSettings.CurrencySettings[promote.LevelUpCost.Currency, default(CompositeIdentificator)].Sprite;
		}).AddTo(disposables);
		promoteButton.onClick.AddListener(Promote);
	}

	private void OnDestroy()
	{
		promoteButton.onClick.RemoveAllListeners();
	}
}
