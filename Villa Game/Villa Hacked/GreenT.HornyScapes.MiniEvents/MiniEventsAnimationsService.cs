using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Content;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsAnimationsService : MonoView
{
	[SerializeField]
	private MiniEventsCurrencyFly _miniEventsCurrencyFly;

	[SerializeField]
	private GameObject _inputBlocker;

	[SerializeField]
	private CurrencyView _currencyView;

	private ContentAdder _contentAdder;

	private Queue<Sequence> _applyAnimationSequence;

	private Queue<IDisposable> _animationDisposables;

	private Queue<Action<int>> _onCurrencyAnimationEndSequence;

	private Queue<int> _onAnimationEndValue;

	private void Awake()
	{
		_applyAnimationSequence = new Queue<Sequence>();
		_animationDisposables = new Queue<IDisposable>();
		_onCurrencyAnimationEndSequence = new Queue<Action<int>>();
		_onAnimationEndValue = new Queue<int>();
	}

	[Inject]
	private void Init(ContentAdder contentAdder)
	{
		_contentAdder = contentAdder;
	}

	public void SetupCurrencyViewInAnimationState()
	{
		_currencyView.IsOnCurrencyAnimation = true;
	}

	public void LaunchCurrencyFlyAnimation(MiniEventTaskItemView taskView, List<CurrencyLinkedContent> currencyRewards)
	{
		_onAnimationEndValue.Enqueue(_currencyView.CurrentValue);
		_onCurrencyAnimationEndSequence.Enqueue(delegate(int currentValue)
		{
			_currencyView.SetText(currentValue);
		});
		for (int i = 0; i < currencyRewards.Count; i++)
		{
			MiniEventTaskCurrencyRewardItemView miniEventTaskCurrencyRewardItemView = taskView.TryGetCurrencyRewardItem(currencyRewards[i].Currency, currencyRewards[i].CompositeIdentificator);
			Sequence sequence = TryGetSequence();
			sequence = _miniEventsCurrencyFly.LaunchCurrency(miniEventTaskCurrencyRewardItemView.transform, currencyRewards[i].Currency, currencyRewards[i].CompositeIdentificator, currencyRewards[i].Quantity);
			Sequence sequence2 = sequence;
			sequence2.onKill = (TweenCallback)Delegate.Combine(sequence2.onKill, (TweenCallback)delegate
			{
				Action<int> action = default(Action<int>);
				if (_onCurrencyAnimationEndSequence.TryDequeue(ref action))
				{
					int obj = _onAnimationEndValue.Dequeue();
					action(obj);
				}
				_currencyView.IsOnCurrencyAnimation = _onCurrencyAnimationEndSequence.Any();
				EnqueSequence(sequence);
			});
		}
	}

	public void LaunchAnyRewardAnimation(LinkedContent reward)
	{
		_contentAdder.AddContent(reward);
	}

	public void LaunchTaskHideAnimation(RectTransformAnimation hideAnimation, Action onKill, double delay = 0.0)
	{
		if (delay > 0.0)
		{
			_inputBlocker.SetActive(value: true);
		}
		hideAnimation.Play();
		IDisposable disposable = TryGetDisposable();
		disposable?.Dispose();
		disposable = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.Delay<Animation>(Observable.FirstOrDefault<Animation>(hideAnimation.OnAnimationEnd), TimeSpan.FromSeconds(delay)), (Action<Animation>)delegate
		{
			try
			{
				onKill();
				EnqueDisposable(disposable);
				_inputBlocker.SetActive(value: false);
			}
			catch (Exception innerException)
			{
				throw innerException.SendException(GetType().Name + " error on complete animation");
			}
		}), (Component)this);
	}

	private IDisposable TryGetDisposable()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		IDisposable result = default(IDisposable);
		if (!_animationDisposables.TryDequeue(ref result))
		{
			return (IDisposable)new CompositeDisposable();
		}
		return result;
	}

	private void EnqueDisposable(IDisposable disposable)
	{
		_animationDisposables.Enqueue(disposable);
	}

	private Sequence TryGetSequence()
	{
		Sequence result = default(Sequence);
		if (!_applyAnimationSequence.TryDequeue(ref result))
		{
			return DOTween.Sequence();
		}
		return result;
	}

	private void EnqueSequence(Sequence sequence)
	{
		_applyAnimationSequence.Enqueue(sequence);
	}
}
