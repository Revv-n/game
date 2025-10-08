using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events.UI;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.Sounds;
using GreenT.UI;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskStarFly : MonoBehaviour
{
	[SerializeField]
	private BezierAnimate bezierAnimate;

	[SerializeField]
	private BezierAnimate jewelBezierAnimate;

	[SerializeField]
	private BezierAnimate eventBezierAnimate;

	[SerializeField]
	private Animation eventXPStretchAnimation;

	[SerializeField]
	private AudioClip StarDing;

	private IWindowsManager windowsManager;

	private IAudioPlayer audioPlayer;

	private ResourcesWindow resourcesWindow;

	private EventCoreUI eventCoreUI;

	[Inject]
	private void Init(IWindowsManager windowsManager, IAudioPlayer audioPlayer)
	{
		this.windowsManager = windowsManager;
		this.audioPlayer = audioPlayer;
	}

	private void Awake()
	{
		resourcesWindow = windowsManager.Get<ResourcesWindow>();
		eventCoreUI = windowsManager.Get<EventCoreUI>();
	}

	public Sequence LaunchStar(Transform startPosition, CurrencyType currency, int count = 1)
	{
		Transform transform = null;
		Sequence result = null;
		if (currency == CurrencyType.EventXP)
		{
			transform = eventCoreUI.EventCurrencySpriteAttacher.transform;
			result = eventBezierAnimate.Launch(startPosition, transform).Append(eventXPStretchAnimation.Play());
		}
		else
		{
			if (resourcesWindow == null)
			{
				return result;
			}
			transform = resourcesWindow.GetCurrencyTransform(currency);
			result = ((currency != CurrencyType.Jewel && currency != CurrencyType.Contracts) ? bezierAnimate.Launch(startPosition, transform, count) : jewelBezierAnimate.Launch(startPosition, transform, count));
		}
		Sequence sequence = result;
		sequence.onStepComplete = (TweenCallback)Delegate.Combine(sequence.onStepComplete, (TweenCallback)delegate
		{
			audioPlayer.PlayOneShotAudioClip2D(StarDing);
		});
		return result;
	}
}
