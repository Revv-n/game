using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tutorial;

public sealed class TutorialButton<TComponent, KStep> : IDisposable where TComponent : BaseTutorialComponent<KStep> where KStep : TutorialStepSO
{
	private Button _button;

	private TutorialHighlighter _tutorialHighlighter;

	private BaseTutorialComponent<KStep> _activeComponent;

	private IDisposable _clickStream;

	private IDisposable _onCompleteStream;

	private readonly CompositeDisposable _onActivateStream = new CompositeDisposable();

	public void Init(TutorialHighlighter tutorialHighlighter, Button button)
	{
		_tutorialHighlighter = tutorialHighlighter;
		_button = button;
	}

	public void ClearActiveStream()
	{
		_onActivateStream.Clear();
	}

	public void SubscribeOnActivate(TComponent component)
	{
		if (component.IsActive)
		{
			OnActivate(component);
		}
		else
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<BaseTutorialComponent<KStep>>(component.OnActivate, (Action<BaseTutorialComponent<KStep>>)OnActivate), (ICollection<IDisposable>)_onActivateStream);
		}
	}

	private void OnActivate(BaseTutorialComponent<KStep> step)
	{
		_onCompleteStream?.Dispose();
		_ = _activeComponent;
		_activeComponent = step;
		try
		{
			_tutorialHighlighter.Activate(step.BlockScreen, step.IsLight);
		}
		catch (Exception innerException)
		{
			innerException.SendException($"{step.GroupID}:{step.StepID}: can't light icon broken");
		}
		_onCompleteStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)step.StepModel.IsComplete, (Func<bool, bool>)((bool _value) => _value)), (Action<bool>)delegate
		{
			OnComplete();
		});
		_clickStream = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_button), (Action<Unit>)delegate
		{
			OnClick();
		});
	}

	private void OnClick()
	{
		_activeComponent.CompleteStep();
		_onCompleteStream.Dispose();
	}

	private void OnComplete()
	{
		_clickStream?.Dispose();
		_tutorialHighlighter.Deactivate();
	}

	public void Dispose()
	{
		_clickStream?.Dispose();
		_onActivateStream.Dispose();
		_onCompleteStream?.Dispose();
	}
}
