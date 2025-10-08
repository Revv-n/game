using System;
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
			component.OnActivate.Subscribe(OnActivate).AddTo(_onActivateStream);
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
		_onCompleteStream = step.StepModel.IsComplete.Where((bool _value) => _value).Subscribe(delegate
		{
			OnComplete();
		});
		_clickStream = _button.OnClickAsObservable().Subscribe(delegate
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
