using System;
using DG.Tweening;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Resources.UI;
using Merge;
using StripClub.Model;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class AddPresentsClip : Clip
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
	private CardResourceView _cardView;

	[SerializeField]
	private TMP_Text _quantity;

	[SerializeField]
	private AnimateCollect _animateCollect;

	[SerializeField]
	private ImagePool _pool;

	[Header("Animation Settings")]
	[SerializeField]
	private float _waitOnTheEnd = 0.3f;

	[SerializeField]
	private float _flyResourcesDelay = 1.8f;

	[SerializeField]
	private GreenT.HornyScapes.Animations.CardAnimation _cardAnimation;

	[Tooltip("Объекты, которые включатся после поворота карточки передней частью")]
	[SerializeField]
	private GameObject[] _generalDecorationObjects;

	[Tooltip("Объекты, которые включатся, пока она повёрнута рубашкой к нам")]
	[SerializeField]
	private GameObject[] _generalDecorationBacksideObjects;

	private GreenT.HornyScapes.GameSettings _gameSettings;

	private SoundController _soundController;

	private CurrencyType _currency;

	[Inject]
	private void InnerInit(GreenT.HornyScapes.GameSettings gameSettings, SoundController soundController)
	{
		_gameSettings = gameSettings;
		_soundController = soundController;
	}

	public void Init(PresentLinkedContent presentContent, ResourcesWindow window)
	{
		_currency = GetCurrencyType(presentContent.Present);
		_cardView.Set(_currency, presentContent.Quantity, presentContent.CompositeIdentificator);
		Transform currencyTransform = window.GetCurrencyTransform(_currency);
		if (currencyTransform != null)
		{
			Sprite sprite = _gameSettings.CurrencySettings[_currency, presentContent.CompositeIdentificator].Sprite;
			_animateCollect.Init(_pool, currencyTransform, _cardView.transform, sprite, kill: false);
		}
		_quantity.text = $"+{presentContent.Quantity}";
		GreenT.HornyScapes.Animations.CardAnimation.Settings settings = new GreenT.HornyScapes.Animations.CardAnimation.Settings(_cardView.gameObject, _generalDecorationObjects, _generalDecorationBacksideObjects, _waitOnTheEnd);
		_cardAnimation.Init(settings);
	}

	public override void Play()
	{
		_cardAnimation.Play().OnComplete(Stop);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.TakeUntilDisable<long>(Observable.Take<long>(Observable.Timer(TimeSpan.FromSeconds(_flyResourcesDelay)), 1), (Component)this), (Action<long>)delegate
		{
			LaunchCollectAnimation();
		}), (Component)this);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		_cardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}

	private void LaunchCollectAnimation()
	{
		if (_currency != CurrencyType.MiniEvent)
		{
			_animateCollect.Launch();
			_soundController.PlayCurrencySound(_currency);
		}
	}

	private CurrencyType GetCurrencyType(Present present)
	{
		if (!int.TryParse(present.Id.Replace("present_", ""), out var result))
		{
			throw new Exception("Invalid present id (" + present.Id);
		}
		return result switch
		{
			1 => CurrencyType.Present1, 
			2 => CurrencyType.Present2, 
			3 => CurrencyType.Present3, 
			4 => CurrencyType.Present4, 
			_ => throw new Exception("Invalid present id (" + present.Id), 
		};
	}
}
