using System;
using DG.Tweening;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Resources.UI;
using GreenT.Types;
using Merge;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class AddResourcesClip : Clip
{
	[Serializable]
	public class TargetsTransformsDictionary : SerializableDictionary<CurrencyType, Transform>
	{
	}

	[Serializable]
	public class TargetsTMProDictionary : SerializableDictionary<CurrencyType, TextMeshProUGUI>
	{
	}

	[SerializeField]
	private CardResourceView cardView;

	[SerializeField]
	private TMP_Text quantity;

	[SerializeField]
	private AnimateCollect animateCollect;

	[SerializeField]
	private ImagePool pool;

	[Header("Animation Settings")]
	[SerializeField]
	private float waitOnTheEnd = 0.3f;

	[SerializeField]
	private float flyResourcesDelay = 1.8f;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] generalDecorationObjects;

	[Tooltip("Объекты, которые включатся пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] generalDecorationBacksideObjects;

	private GreenT.HornyScapes.GameSettings gameSettings;

	private SoundController soundController;

	private CurrencyType currency;

	[Inject]
	private void InnerInit(GreenT.HornyScapes.GameSettings gameSettings, SoundController soundController)
	{
		this.gameSettings = gameSettings;
		this.soundController = soundController;
	}

	public void Init(CurrencyLinkedContent currencyContent, ResourcesWindow window)
	{
		CompositeIdentificator compositeIdentificator = currencyContent.CompositeIdentificator;
		CurrencyType currencyType = (currency = currencyContent.Currency);
		cardView.Set(currencyType, currencyContent.Quantity, compositeIdentificator);
		Transform currencyTransform = window.GetCurrencyTransform(currencyType);
		if (currencyTransform != null)
		{
			Sprite rewardIcon = null;
			if (gameSettings.CurrencySettings.Contains(currencyType, compositeIdentificator))
			{
				rewardIcon = gameSettings.CurrencySettings[currencyType, compositeIdentificator].Sprite;
			}
			else
			{
				string text = currencyType.ToString();
				CompositeIdentificator compositeIdentificator2 = compositeIdentificator;
				Debug.LogError("Dictionary gameSettings.CurrencySettings doesn't contains currency type: " + text + " identificator = " + compositeIdentificator2.ToString());
			}
			animateCollect.Init(pool, currencyTransform, cardView.transform, rewardIcon, kill: false);
		}
		quantity.text = "+" + currencyContent.Quantity;
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(cardView.gameObject, generalDecorationObjects, generalDecorationBacksideObjects, waitOnTheEnd);
		cardAnimation.Init(settings);
	}

	public override void Play()
	{
		cardAnimation.Play().OnComplete(Stop);
		Observable.Timer(TimeSpan.FromSeconds(flyResourcesDelay)).Take(1).TakeUntilDisable(this)
			.Subscribe(delegate
			{
				LaunchCollectAnimation();
			})
			.AddTo(this);
	}

	private void LaunchCollectAnimation()
	{
		if (currency != CurrencyType.MiniEvent)
		{
			animateCollect.Launch();
			soundController.PlayCurrencySound(currency);
		}
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}
}
