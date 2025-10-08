using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using UnityEngine;

public sealed class TutorialPresentClickCountSystem : TutorialEntitySystem<TutorialClickCountComponent, TutorialClickCountStepSO>
{
	[SerializeField]
	private PresentsWindowSetter _presentsWindowSetter;

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
		_presentsWindowSetter.PresentViewsShowed.Subscribe(SubscribeInteract).AddTo(_disposables);
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
		_disposables?.Dispose();
	}

	private void SubscribeInteract(IReadOnlyList<PresentView> presentViews)
	{
		Clear();
		foreach (PresentView presentView in presentViews)
		{
			string id = presentView.Source.Id;
			if (TryGetComponent(id, out var tutorialEntityComponent))
			{
				SubscribeOnActivate(tutorialEntityComponent, presentView);
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

	private void SubscribeOnActivate(TutorialClickCountComponent component, PresentView view)
	{
		if (component.IsActive)
		{
			OnActivate(component, view);
			return;
		}
		component.OnActivate.Subscribe(delegate
		{
			OnActivate(component, view);
		}).AddTo(_disposables);
	}

	private void OnActivate(BaseTutorialComponent<TutorialClickCountStepSO> step, PresentView view)
	{
		_onCompleteStream?.Dispose();
		_ = _activeComponent;
		_activeComponent = step;
		_onCompleteStream = step.StepModel.IsComplete.Where((bool value) => value).Subscribe(delegate
		{
			_clickStream?.Dispose();
		}).AddTo(_disposables);
		_clickStream = view.PointerDown.Subscribe(delegate
		{
			CheckClickCount();
		}).AddTo(_disposables);
	}

	private void Clear()
	{
		_targetClickCount = 0;
		_currentClickCount = 0;
	}

	private void CheckClickCount()
	{
		_currentClickCount++;
		if (_currentClickCount >= _targetClickCount)
		{
			_activeComponent.CompleteStep();
			_onCompleteStream.Dispose();
		}
	}
}
