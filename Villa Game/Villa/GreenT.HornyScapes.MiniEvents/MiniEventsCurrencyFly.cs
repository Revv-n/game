using System;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.Sounds;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsCurrencyFly : MonoBehaviour
{
	[SerializeField]
	private MiniEventsBezierAnimator _bezierAnimate;

	[SerializeField]
	private MiniEventsBezierAnimator _minieventBezierAnimate;

	[SerializeField]
	private MiniEventsBezierAnimator _minieventCurrentBezierAnimate;

	[SerializeField]
	private AudioClip _starDing;

	[SerializeField]
	private Transform _currentOpenedCurrency;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation _stretchAnimation;

	private IWindowsManager _windowsManager;

	private IAudioPlayer _audioPlayer;

	private ResourcesWindow _resourcesWindow;

	private MiniEventViewControllerService _miniEventViewControllerService;

	private MiniEventSettingsProvider _miniEventSettingsProvider;

	[Inject]
	private void Init(IWindowsManager windowsManager, IAudioPlayer audioPlayer, MiniEventViewControllerService miniEventViewControllerService, MiniEventSettingsProvider miniEventSettingsProvider)
	{
		_windowsManager = windowsManager;
		_audioPlayer = audioPlayer;
		_miniEventViewControllerService = miniEventViewControllerService;
		_miniEventSettingsProvider = miniEventSettingsProvider;
	}

	private void Awake()
	{
		_resourcesWindow = _windowsManager.Get<ResourcesWindow>();
	}

	public Sequence LaunchCurrency(Transform startPosition, CurrencyType currency, CompositeIdentificator currencyIdentificator, int count = 1)
	{
		Transform transform = null;
		Sequence sequence = null;
		if (currency == CurrencyType.MiniEvent)
		{
			if (_miniEventSettingsProvider.GetEvent(_miniEventViewControllerService.CurrentMiniEvent.EventId).CurrencyIdentificator == currencyIdentificator)
			{
				transform = _currentOpenedCurrency;
				sequence = _minieventCurrentBezierAnimate.Launch(startPosition, transform, currency, currencyIdentificator).Append(_stretchAnimation.Play());
			}
			else
			{
				_miniEventSettingsProvider.Collection.FirstOrDefault((MiniEvent minievent) => minievent.CurrencyIdentificator == currencyIdentificator);
				transform = _miniEventViewControllerService.GetMiniEventViewTransformByCurrencyIdentificator(currencyIdentificator);
				sequence = _minieventCurrentBezierAnimate.Launch(startPosition, transform, currency, currencyIdentificator).Append(_stretchAnimation.Play());
			}
		}
		else
		{
			transform = _resourcesWindow.GetCurrencyTransform(currency);
			sequence = _bezierAnimate.Launch(startPosition, transform, currency, currencyIdentificator, count);
		}
		Sequence sequence2 = sequence;
		sequence2.onStepComplete = (TweenCallback)Delegate.Combine(sequence2.onStepComplete, (TweenCallback)delegate
		{
			_audioPlayer.PlayOneShotAudioClip2D(_starDing);
		});
		return sequence;
	}
}
