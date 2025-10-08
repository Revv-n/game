using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.External.GreenT.Utilities;
using GreenT.Types;
using StripClub.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events.UI;

public class RewardIcon : MonoBehaviour
{
	[SerializeField]
	private Image currentIcon;

	[SerializeField]
	private CanvasGroup currentIconCanvas;

	[SerializeField]
	private Image nextIcon;

	[SerializeField]
	private Animation updateIconAnimation;

	private readonly CompositeDisposable currencyChangeStream = new CompositeDisposable();

	private EventRewardTracker _eventRewardTracker;

	private GameSettings _gameSettings;

	private IDisposable _iconChangeStream;

	[Inject]
	private void InnerInit(GameSettings gameSettings, EventRewardTracker eventRewardTracker)
	{
		_gameSettings = gameSettings;
		_eventRewardTracker = eventRewardTracker;
	}

	private void Start()
	{
		nextIcon.gameObject.SetActive(value: false);
		currentIcon.gameObject.SetActive(value: true);
		BaseReward value = _eventRewardTracker.Target.Value;
		Sprite sprite = value?.Content.GetProgressBarIcon();
		if (value?.Content is CurrencyLinkedContent currencyLinkedContent)
		{
			ApplyData(currentIcon, sprite, currencyLinkedContent.Currency, currencyLinkedContent.CompositeIdentificator);
		}
	}

	private void SubscribeRewardIcon(Image image, CurrencyLinkedContent currencyLinkedContent)
	{
		_iconChangeStream?.Dispose();
		CurrencyType currency = currencyLinkedContent.Currency;
		ApplyData(image, _gameSettings.CurrencySettings[currency, currencyLinkedContent.CompositeIdentificator].AlternativeSprite, currency, currencyLinkedContent.CompositeIdentificator);
		_iconChangeStream = ObservableExtensions.Subscribe<Sprite>(ObserveExtensions.ObserveEveryValueChanged<CurrencySettings, Sprite>(_gameSettings.CurrencySettings[currency, currencyLinkedContent.CompositeIdentificator], (Func<CurrencySettings, Sprite>)((CurrencySettings actualSettings) => actualSettings.AlternativeSprite), (FrameCountType)0, false), (Action<Sprite>)delegate(Sprite sprite)
		{
			ApplyData(image, sprite, currency, currencyLinkedContent.CompositeIdentificator);
		});
	}

	private void ApplyData(Image image, Sprite sprite, CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		image.sprite = ((sprite == null) ? _gameSettings.CurrencyPlaceholder[currencyType, currencyIdentificator].AlternativeSprite : sprite);
	}

	public void OnEnable()
	{
		TrackIcon();
	}

	private void TrackIcon()
	{
		currencyChangeStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<EventReward>(Observable.Select<BaseReward, EventReward>(Observable.Where<BaseReward>((IObservable<BaseReward>)_eventRewardTracker.Target, (Func<BaseReward, bool>)((BaseReward x) => x != null)), (Func<BaseReward, EventReward>)((BaseReward x) => x as EventReward)), (Action<EventReward>)SetNextRewardIcon), (ICollection<IDisposable>)currencyChangeStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.SelectMany<BaseReward, Animation>(Observable.Do<BaseReward>(Observable.Skip<BaseReward>((IObservable<BaseReward>)_eventRewardTracker.Target, 1), (Action<BaseReward>)delegate
		{
			updateIconAnimation.Play();
		}), Observable.FirstOrDefault<Animation>(updateIconAnimation.OnAnimationEnd)), (Action<Animation>)delegate
		{
			UpdateCurrentIcon();
		}), (ICollection<IDisposable>)currencyChangeStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<CurrencyLinkedContent>(Observable.Where<CurrencyLinkedContent>(Observable.Select<BaseReward, CurrencyLinkedContent>(Observable.Where<BaseReward>((IObservable<BaseReward>)_eventRewardTracker.Target, (Func<BaseReward, bool>)((BaseReward value) => value != null)), (Func<BaseReward, CurrencyLinkedContent>)((BaseReward value) => value.Content as CurrencyLinkedContent)), (Func<CurrencyLinkedContent, bool>)((CurrencyLinkedContent content) => content != null)), (Action<CurrencyLinkedContent>)delegate(CurrencyLinkedContent value)
		{
			SubscribeRewardIcon(nextIcon, value);
		}), (ICollection<IDisposable>)currencyChangeStream);
	}

	private void UpdateCurrentIcon()
	{
		currentIcon.sprite = nextIcon.sprite;
		currentIcon.transform.SetTransform(nextIcon.transform);
		if (currentIconCanvas != null)
		{
			currentIconCanvas.alpha = 1f;
		}
		currentIcon.gameObject.SetActive(value: true);
		nextIcon.gameObject.SetActive(value: false);
	}

	private void SetNextRewardIcon(EventReward eventReward)
	{
		Sprite progressBarIcon = eventReward.Content.GetProgressBarIcon();
		nextIcon.sprite = progressBarIcon;
		if (eventReward.Content is CurrencyLinkedContent currencyLinkedContent)
		{
			ApplyData(currentIcon, progressBarIcon, currencyLinkedContent.Currency, currencyLinkedContent.CompositeIdentificator);
		}
		else
		{
			currentIcon.sprite = progressBarIcon;
		}
		nextIcon.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		currencyChangeStream.Clear();
	}

	private void OnDestroy()
	{
		_iconChangeStream?.Dispose();
		currencyChangeStream.Dispose();
	}
}
