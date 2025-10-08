using System;
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
	private GreenT.HornyScapes.Animations.Animation updateIconAnimation;

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
		_iconChangeStream = _gameSettings.CurrencySettings[currency, currencyLinkedContent.CompositeIdentificator].ObserveEveryValueChanged((CurrencySettings actualSettings) => actualSettings.AlternativeSprite).Subscribe(delegate(Sprite sprite)
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
		(from x in _eventRewardTracker.Target
			where x != null
			select x as EventReward).Subscribe(SetNextRewardIcon).AddTo(currencyChangeStream);
		_eventRewardTracker.Target.Skip(1).Do(delegate
		{
			updateIconAnimation.Play();
		}).SelectMany(updateIconAnimation.OnAnimationEnd.FirstOrDefault())
			.Subscribe(delegate
			{
				UpdateCurrentIcon();
			})
			.AddTo(currencyChangeStream);
		(from value in _eventRewardTracker.Target
			where value != null
			select value.Content as CurrencyLinkedContent into content
			where content != null
			select content).Subscribe(delegate(CurrencyLinkedContent value)
		{
			SubscribeRewardIcon(nextIcon, value);
		}).AddTo(currencyChangeStream);
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
