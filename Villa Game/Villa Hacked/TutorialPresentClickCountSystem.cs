using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IReadOnlyList<PresentView>>(_presentsWindowSetter.PresentViewsShowed, (Action<IReadOnlyList<PresentView>>)SubscribeInteract), (ICollection<IDisposable>)_disposables);
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseTutorialComponent<TutorialClickCountStepSO>>(component.OnActivate, (Action<BaseTutorialComponent<TutorialClickCountStepSO>>)delegate
		{
			OnActivate(component, view);
		}), (ICollection<IDisposable>)_disposables);
	}

	private void OnActivate(BaseTutorialComponent<TutorialClickCountStepSO> step, PresentView view)
	{
		_onCompleteStream?.Dispose();
		_ = _activeComponent;
		_activeComponent = step;
		_onCompleteStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)step.StepModel.IsComplete, (Func<bool, bool>)((bool value) => value)), (Action<bool>)delegate
		{
			_clickStream?.Dispose();
		}), (ICollection<IDisposable>)_disposables);
		_clickStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PointerEventData>(view.PointerDown, (Action<PointerEventData>)delegate
		{
			CheckClickCount();
		}), (ICollection<IDisposable>)_disposables);
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
