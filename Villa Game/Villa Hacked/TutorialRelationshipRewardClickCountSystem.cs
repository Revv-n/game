using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Relationships.Views;
using GreenT.HornyScapes.Relationships.Windows;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class TutorialRelationshipRewardClickCountSystem : TutorialEntitySystem<TutorialClickCountComponent, TutorialClickCountStepSO>
{
	[SerializeField]
	private RelationshipUiSetter _relationshipUiSetter;

	[SerializeField]
	private ScrollRect _scroll;

	private Button _button;

	private int _targetClickCount;

	private int _currentClickCount;

	private BaseTutorialComponent<TutorialClickCountStepSO> _activeComponent;

	private IDisposable _clickStream;

	private IDisposable _onCompleteStream;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public List<TutorialClickCountStepSO> Steps = new List<TutorialClickCountStepSO>();

	public override bool TryInitSystem()
	{
		if (!base.TryInitSystem())
		{
			return false;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IReadOnlyList<BaseRewardView>>(_relationshipUiSetter.RewardsShowed, (Action<IReadOnlyList<BaseRewardView>>)SubscribeInteract), (ICollection<IDisposable>)_disposables);
		return true;
	}

	protected override void InitComponents()
	{
		foreach (TutorialClickCountStepSO step in Steps)
		{
			TutorialClickCountComponent tutorialClickCountComponent = new TutorialClickCountComponent(step, groupManager);
			if (tutorialClickCountComponent.IsInited.Value)
			{
				components.Add(tutorialClickCountComponent);
			}
		}
	}

	protected override void InitSubSystem()
	{
	}

	protected override void DestroySubSystem()
	{
		Clear();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Clear();
		_clickStream?.Dispose();
		_onCompleteStream?.Dispose();
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
	}

	private void SubscribeInteract(IReadOnlyList<BaseRewardView> rewardViews)
	{
		Clear();
		foreach (BaseRewardView rewardView in rewardViews)
		{
			string id = rewardView.Source.Id.ToString();
			if (TryGetComponent(id, out var tutorialEntityComponent))
			{
				if (rewardView.gameObject.TryGetComponent<TutorialTargetButton>(out var component))
				{
					_button = component.Button;
				}
				else if (!rewardView.gameObject.TryGetComponent<Button>(out _button))
				{
					_button = rewardView.gameObject.AddComponent<Button>();
				}
				SubscribeOnActivate(tutorialEntityComponent);
				_targetClickCount = tutorialEntityComponent.Count;
				_currentClickCount = 0;
				break;
			}
		}
	}

	private bool TryGetComponent(string id, out TutorialClickCountComponent tutorialEntityComponent)
	{
		tutorialEntityComponent = components.Find((TutorialClickCountComponent component) => component.Id.Equals(id));
		return tutorialEntityComponent != null;
	}

	private void SubscribeOnActivate(TutorialClickCountComponent component)
	{
		if (component.IsActive)
		{
			OnActivate(component);
		}
		else
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseTutorialComponent<TutorialClickCountStepSO>>(component.OnActivate, (Action<BaseTutorialComponent<TutorialClickCountStepSO>>)OnActivate), (ICollection<IDisposable>)_disposables);
		}
	}

	private void OnActivate(BaseTutorialComponent<TutorialClickCountStepSO> step)
	{
		_onCompleteStream?.Dispose();
		_ = _activeComponent;
		_activeComponent = step;
		_onCompleteStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)step.StepModel.IsComplete, (Func<bool, bool>)((bool _value) => _value)), (Action<bool>)delegate
		{
			_clickStream?.Dispose();
		});
		_clickStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_button), (Action<Unit>)delegate
		{
			CheckClickCount();
		});
		if (_scroll != null)
		{
			_scroll.enabled = false;
		}
	}

	private void Clear()
	{
		_button = null;
		_targetClickCount = 0;
		_currentClickCount = 0;
	}

	private void CheckClickCount()
	{
		_currentClickCount++;
		if (_currentClickCount >= _targetClickCount)
		{
			if (_scroll != null)
			{
				_scroll.enabled = true;
			}
			_activeComponent.CompleteStep();
			_onCompleteStream.Dispose();
		}
	}
}
